import sys
import nltk
import os

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))

nltk.download("en_core_web_lg")
nltk.data.path.append(SCRIPT_DIR)
