# Install these if not already installed
# !pip install textblob googletrans==4.0.0-rc1
# python -m textblob.download_corpora

nltk.download('punkt')

from textblob import TextBlob
# !pip install googletrans==4.0.0-rc1


from googletrans import Translator

# Initialize translator
translator = Translator()

print("Please upload your JSON file containing monument data.")
uploaded = files.upload()  # This will prompt file upload

# Ensure a file was uploaded
if not uploaded:
    raise ValueError("No file was uploaded. Please upload a JSON file.")

# Get the uploaded file name
filename = next(iter(uploaded))

# Step 2: Load JSON Data
with open(filename, "r", encoding="utf-8") as file:
    monuments = json.load(file)

# Convert to a dictionary for easy lookup
# Key: Lowercase name of the monument
# Value: The original description
monument_dict = {m["MonumentName"].lower(): m["MonumentDescription"] for m in monuments}

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
    Tries to extract the monument name from the user input and returns
    the description if found. Otherwise returns an apology message.
    """
    monument_name = extract_monument_name(user_input)
    if monument_name:
        return monument_dict[monument_name]
    else:
        return "Sorry, I don't have information about that monument."

def detect_and_correct(user_text):
    """
    1. Detect the language of user_text using googletrans.
    2. If the language is English, apply TextBlob correction.
    3. If the language is not English, translate it to English
       (without correction, since correction is only for English here).
    4. Return (corrected_or_translated_text, language_code).
    """
    detection = translator.detect(user_text)
    detected_lang = detection.lang

    # If it's English, we'll do a basic TextBlob correction:
    if detected_lang == 'en':
        corrected_text = str(TextBlob(user_text).correct())
        return corrected_text, detected_lang
    else:
        # For non-English, just translate to English
        translated_text = translator.translate(user_text, src=detected_lang, dest='en').text
        return translated_text, detected_lang

def translate_back(text, target_lang):
    """
    Translate a given English text text back to the language target_lang.
    If target_lang is already English, just return the text as is.
    """
    if target_lang.lower().startswith('en'):
        return text
    else:
        return translator.translate(text, src='en', dest=target_lang).text

def chatbot():
    print("Chatbot is ready! Ask about monuments in Sbiba. Type 'exit' to quit.")

    while True:
        user_input = input("You: ").strip()
        if user_input.lower() in ["exit", "quit", "bye"]:
            print("Goodbye! Have a great day!")
            break

        # 1. Detect language & correct/translate to English
        corrected_english, user_lang = detect_and_correct(user_input)

        # 2. Extract info
        response_english = get_monument_info(corrected_english)

        # 3. Translate the response back to original user language
        final_response = translate_back(response_english, user_lang)

        print(f"Chatbot: {final_response}")

# Run the chatbot
chatbot()