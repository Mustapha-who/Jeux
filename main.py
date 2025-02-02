from fastapi import FastAPI, HTTPException, Header
from pydantic import BaseModel
import json
import nltk
from textblob import TextBlob
from googletrans import Translator

# Download necessary data
nltk.download('punkt')

# Initialize FastAPI
app = FastAPI()

# Set an API key (Replace with a secure method like env variables in production)
API_KEY = "my_secure_api_key_123"

# Initialize Translator
translator = Translator()

# Load JSON data (Replace 'monuments.json' with your actual file)
with open("monuments.json", "r", encoding="utf-8") as file:
    monuments = json.load(file)

# Convert JSON to a dictionary for easy lookup
monument_dict = {m["MonumentName"].lower(): m["MonumentDescription"] for m in monuments}

# Define request body model
class ChatRequest(BaseModel):
    message: str

from difflib import get_close_matches

def extract_monument_name(user_input):
    """
    Extracts a monument name from user input using fuzzy matching.
    """
    user_input = user_input.lower()
    
    # Check for exact matches
    for monument in monument_dict.keys():
        if monument in user_input:
            return monument  # Return matched monument name
    
    # Try fuzzy matching for partial matches
    close_matches = get_close_matches(user_input, monument_dict.keys(), n=1, cutoff=0.5)
    if close_matches:
        return close_matches[0]  # Return the best matching monument name
    
    return None  # No match found

def get_monument_info(user_input):
    """
    Retrieves monument information based on user input.
    """
    monument_name = extract_monument_name(user_input)
    if monument_name:
        return monument_dict[monument_name]
    else:
        return "Sorry, I don't have information about that monument."

async def detect_and_correct(user_text):
    """
    Detects language, corrects spelling (if English), or translates to English.
    """
    detection = await translator.detect(user_text)  # Await the async call
    detected_lang = detection.lang

    if detected_lang == 'en':
        corrected_text = str(TextBlob(user_text).correct())
        return corrected_text, detected_lang
    else:
        translated_text = await translator.translate(user_text, src=detected_lang, dest='en')
        return translated_text.text, detected_lang

async def translate_back(text, target_lang):
    """
    Translates the chatbot response back to the original language.
    """
    if target_lang.lower().startswith('en'):
        return text
    else:
        translated_text = await translator.translate(text, src='en', dest=target_lang)
        return translated_text.text

@app.post("/chat")
async def chat(request: ChatRequest, api_key: str = Header(None)):
    """
    API endpoint to receive a message and return a chatbot response.
    Requires a valid API key in the request header.
    """
    # ✅ Step 1: Check API key
    if api_key != API_KEY:
        raise HTTPException(status_code=403, detail="Invalid API Key")

    user_input = request.message.strip()

    # ✅ Step 2: Detect language & correct/translate to English
    corrected_english, user_lang = await detect_and_correct(user_input)

    # ✅ Step 3: Extract monument info
    response_english = get_monument_info(corrected_english)

    # ✅ Step 4: Translate response back to original language
    final_response = await translate_back(response_english, user_lang)

    return {"response": final_response}