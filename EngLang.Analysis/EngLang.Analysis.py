import sys
import argparse
import nltk
import spacy
from spacy.language import Language
from nltk.tree import Tree

nlp: Language = spacy.load("en_core_web_trf")

@Language.component("custom_sentence_boundaries")
def set_custom_boundaries(doc):
    for token in doc[:-1]:
        if token.text == ":" or token.text == "):" or token.text == "***" or token.text == ";":
            doc[token.i+1].is_sent_start = True
        #elif token.text == "Rs." or token.text == ")":
        #    doc[token.i+1].is_sent_start = False
    return doc

nlp.add_pipe("custom_sentence_boundaries", name="custom_sentence_boundaries", before="parser")

# download required nltk packages
# required for tokenization
nltk.download('punkt')
# required for parts of speech tagging
nltk.download('averaged_perceptron_tagger')

simple_grammar = nltk.CFG.fromstring("""
      S -> NP VP '.'
      PP -> P NP
      NP -> Det N | Det N PP
      VP -> V NP | VP PP
      Det -> 'DT'
      N -> 'NN'
      V -> 'VB'
      P -> 'PP'
      """)
grammar_parser = nltk.ChartParser(simple_grammar)
"""
Visualize the SpaCy dependency tree with nltk.tree
"""
def token_format(token):
    return "_".join([token.orth_, token.tag_, token.dep_])

def to_nltk_tree(node):
    if node.n_lefts + node.n_rights > 0:
        return Tree(token_format(node),
                    [to_nltk_tree(child)
                    for child in node.children]
                )
    else:
        return Tree(token_format(node), [])

def nltk_spacy_tree(sent):
    #tree = [to_nltk_tree(sent.root) for sent in doc.sents]
    # The first item in the list is the full tree
    return to_nltk_tree(sent.root)#.draw()

def tokenize_sentence(sentence):
    return nltk.word_tokenize(sentence)

def analyse_sentence_spacy(sentence):
    print('spacy')
    doc = nlp(sentence)
    #print ([(token.text, token.pos_) for token in doc])
    assert doc.has_annotation("SENT_START")
    tagged_tokens = []
    for sent in doc.sents:
        print(sent.text)
        nltk_spacy_tree(sent).pretty_print()
        tagged_tokens.append([(token.text, token.tag_) for token in sent])

    # print tagged tokens
    print('[')
    for l in tagged_tokens:
        print('   ', l)
    print(']')

    dump_tokens_nltk(tagged_tokens)

def normalize_token(token, pos):
    if (pos == '_SP'):
        return pos
    return pos[:2]

def dump_tokens_nltk(tagged_tokens: list[list[tuple[any,str]]]):
    pos_tags = [[normalize_token(token, pos) for (token,pos) in sent] for sent in tagged_tokens]
    #pos_tags = sum(pos_tags,[])
    print(list(set(sum(pos_tags,[]) )))

def analyse_sentence_nltk(sentence):
    print('nltk')
    tokens = [tokenize_sentence(t) for t in nltk.sent_tokenize(sentence)]

    # parts of speech tagging
    tagged_tokens = nltk.pos_tag_sents(tokens)

    # print tagged tokens
    print('[')
    for l in tagged_tokens:
        print('   ', l)
    print(']')

    dump_tokens_nltk(tagged_tokens)

def analyse_sentence(sentence):
    analyse_sentence_nltk(sentence)
    analyse_sentence_spacy(sentence)

def sample():
    # input text
    sentence1 = """To Calculate factorial of a number: If a number is 0 then result is 1.
    If a number is 1 then result is 1.
    Let a previous number is a number minus 1.
    Calculate factorial of a previous number into a previous factorial.
    Result is a previous factorial multiply a number."""
    analyse_sentence(sentence1)

    sentence2 = """let a value equals 10. Add 20 to a value. multiply a value by 42."""
    analyse_sentence(sentence2)

    sentence2 = """add 20 to a value."""
    analyse_sentence(sentence2)

    sentence2 = """multiply a value by 42."""
    analyse_sentence(sentence2)

    sentence3 = """To get answer to all questions: result is 42."""
    analyse_sentence(sentence3)

    sentence4 = """To calculate rectangle's area from a width and a height: multiply a width by a height."""
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

"""
Types of Parts or speech which is used.
['#', "''", '(', ')', ',', '.', ':', '``',
	'CC',
	'CD',
	'DT',
	'EX',
	'IN',
	'JJ', 'JJR', 'JJS',
	'MD',
	'NN', 'NNP', 'NNS',
	'POS',
	'PRP',
	'RB', 'RBR',
	'RP',
	'SYM', ?? what is this
	'TO',
	'UH', ?? where it occurs
	'VB', 'VBD', 'VBG', 'VBN', 'VBP', 'VBZ',
	'WDT', 'WP', 'WRB']
"""

def analyze_file(filename):
    with open(filename, "r+", encoding="utf-8") as program_file:
        # Reading from a file
        program_content = program_file.read()
        analyse_sentence(program_content)

def split_sentence_spacy(filename, output_file):
    print("split_sentence_spacy " + filename)
    with open(filename, "r+", encoding="utf-8", errors = 'ignore') as program_file:
        # Reading from a file
        program_content = program_file.read()
        content = program_content.splitlines()

        i = 0
        for line in iter(content):
          if line.find('\\') >= 0:
            content[i] = line[:line.find('\\')]
          else:
            content[i] = line
          i += 1

        program_content = '\n'.join(content)

        doc = nlp(program_content)
        #print ([(token.text, token.pos_) for token in doc])
        assert doc.has_annotation("SENT_START")
        for sent in doc.sents:
            print(sent.text.strip())
            print('========================')

arg_parser = argparse.ArgumentParser(
    prog = 'ProgramName',
    description = 'What the program does',
    epilog = 'Text at the bottom of help')
arg_parser.add_argument('command', nargs='?', default = 'sample')
arg_parser.add_argument('-f', '--filename')
arg_parser.add_argument('-o', '--output', required = False)
arg_parser.add_argument('-v', '--verbose',
                    action='store_true')  # on/off flag

args = arg_parser.parse_args()
print(args.command, args.filename)

match args.command:
    case "sample":
        sample()
    case "analyze":
        filename = args.filename
        analyze_file(filename)
    case "sentencize":
        filename = args.filename
        output_file = args.output if args.output else args.filename +'.sentence'
        split_sentence_spacy(filename, output_file)
