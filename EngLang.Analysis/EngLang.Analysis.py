import nltk

# download required nltk packages
# required for tokenization
nltk.download('punkt')
# required for parts of speech tagging
nltk.download('averaged_perceptron_tagger')

def analyse_sentence(sentence):
    # tokene into words
    tokens = [nltk.word_tokenize(t) for t in nltk.sent_tokenize(sentence)]

    # parts of speech tagging
    tagged = nltk.pos_tag_sents(tokens)

    # print tagged tokens
    print('[')
    for l in tagged:
        print('   ', l)
    print(']')

# input text
sentence1 = """To Calculate factorial of a number: if a number is 0 then result is 1.
if a number is 1 then result is 1.
let a previous number is a number minus 1.
calculate factorial of a previous number into a previous factorial.
result is a previous factorial multiply a number."""
analyse_sentence(sentence1)

sentence2 = """let a value equals 10 .
add 20 to a value.
multiply a value by 42."""
analyse_sentence(sentence2)

sentence2 = """add 20 to a value."""
analyse_sentence(sentence2)

sentence2 = """multiply a value by 42."""
analyse_sentence(sentence2)

sentence3 = """To get answer to all questions: result is 42."""
analyse_sentence(sentence3)

sentence4 = """To calculate area from a width and a height: multiply a width by a height."""
analyse_sentence(sentence4)

"""
[
    [('To', 'TO'), ('Calculate', 'NNP'), ('factorial', 'NN'), ('of', 'IN'), ('a', 'DT'), ('number', 'NN'),
        (':', ':'),
        ('if', 'IN'), ('a', 'DT'), ('number', 'NN'), ('is', 'VBZ'), ('0', 'CD'), ('then', 'RB'), ('result', 'NN'), ('is', 'VBZ'), ('1.', 'CD'), ('if', 'IN'), ('a', 'DT'), ('number', 'NN'), ('is', 'VBZ'), ('1', 'CD'), ('then', 'RB'), ('result', 'NN'), ('is', 'VBZ'), ('1.', 'CD'), ('let', 'NN'), ('a', 'DT'), ('previous', 'JJ'), ('number', 'NN'), ('is', 'VBZ'), ('a', 'DT'), ('number', 'NN'), ('minus', 'CC'), ('1.', 'CD'), ('calculate', 'JJ'), ('factorial', 'NN'), ('of', 'IN'), ('a', 'DT'), ('previous', 'JJ'), ('number', 'NN'), ('into', 'IN'), ('a', 'DT'), ('previous', 'JJ'), ('factorial', 'NN'), ('.', '.')]
    [('result', 'NN'), ('is', 'VBZ'), ('a', 'DT'), ('previous', 'JJ'), ('factorial', 'JJ'), ('multiply', 'NN'), ('a', 'DT'), ('number', 'NN'), ('.', '.')]
]
[
    [('let', 'VB'), ('a', 'DT'), ('value', 'NN'), ('equals', 'VBZ'), ('10', 'CD'), ('.', '.')]
    [('add', 'RB'), ('20', 'CD'), ('to', 'TO'), ('a', 'DT'), ('value', 'NN'), ('.', '.')]
    [('multiply', 'VB'), ('a', 'DT'), ('value', 'NN'), ('by', 'IN'), ('42', 'CD'), ('.', '.')]
]
[
    [('add', 'RB'), ('20', 'CD'), ('to', 'TO'), ('a', 'DT'), ('value', 'NN'), ('.', '.')]
]
[
    [('multiply', 'VB'), ('a', 'DT'), ('value', 'NN'), ('by', 'IN'), ('42', 'CD'), ('.', '.')]
]
[
    [('To', 'TO'), ('get', 'VB'), ('answer', 'JJR'), ('to', 'TO'), ('all', 'DT'), ('questions', 'NNS'), (':', ':'), ('result', 'NN'), ('is', 'VBZ'), ('42', 'CD'), ('.', '.')]
]
[
    [('To', 'TO'), ('calculate', 'VB'), ('area', 'NN'), ('from', 'IN'), ('a', 'DT'), ('width', 'NN'), ('and', 'CC'), ('a', 'DT'), ('height', 'NN'), (':', ':'), ('multiply', 'VB'), ('a', 'DT'), ('width', 'NN'), ('by', 'IN'), ('a', 'DT'), ('height', 'NN'), ('.', '.')]
]
"""

"""
To summarize findings
Identifier Reference:
DT + NN
DT + JJ + NN

Function calls starts with VB

Sentence 'result is a previous factorial multiply a number.' seems to be POS tagged incorrectly.
"""

"""
CC   Coordinating Conjunction
CD   Cardinal Digit
DT   Determiner
EX   Existential There. Example: “there is” … think of it like “there exists”)
FW   Foreign Word.
IN   Preposition/Subordinating Conjunction.
JJ   Adjective.
JJR  Adjective, Comparative.
JJS  Adjective, Superlative.
LS   List Marker 1.
MD   Modal.
NN   Noun, Singular.
NNS  Noun Plural.
NNP  Proper Noun, Singular.
NNPS Proper Noun, Plural.
PDT  Predeterminer.
POS  Possessive Ending. Example: parent’s
PRP  Personal Pronoun. Examples: I, he, she
PRP$ Possessive Pronoun. Examples: my, his, hers
RB   Adverb. Examples: very, silently,
RBR  Adverb, Comparative. Example: better
RBS  Adverb, Superlative. Example: best
RP   Particle. Example: give up
TO   to. Example: go ‘to’ the store.
UH   Interjection. Example: errrrrrrrm
VB   Verb, Base Form. Example: take
VBD  Verb, Past Tense. Example: took
VBG  Verb, Gerund/Present Participle. Example: taking
VBN  Verb, Past Participle. Example: taken
VBP  Verb, Sing Present, non-3d take
VBZ  Verb, 3rd person sing. present takes
WDT  wh-determiner. Example: which
WP   wh-pronoun. Example: who, what
WP$  possessive wh-pronoun. Example: whose
WRB  wh-abverb. Example: where, when
"""
