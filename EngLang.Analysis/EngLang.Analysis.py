import sys
import argparse
import nltk
import spacy
import stanza
from spacy.language import Language
from nltk.tree import Tree

nlp: Language = spacy.load("en_core_web_lg")

stanza.download('en', logging_level='WARN')
stanza = stanza.Pipeline(lang='en', processors='tokenize,pos')


process_spacy = True
process_nltk = False
process_stanza = True
print_sentence_tree = False
detect_grammar = False
print_tagged_tokens = True
print_nltk_tokens = False

@Language.component("custom_sentence_boundaries")
def set_custom_boundaries(doc):
    for token in doc[:-1]:
        if token.text == ":" or token.text == "):" or token.text == "***" or token.text == ";":
            doc[token.i+1].is_sent_start = True
        if token.text == "." and (doc[token.i+1].text == ''):
            token.is_sent_end = True
            token.tag_ = '.'
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
      S -> NP VP SentenceEnd | TO VP SentenceEnd
      PP -> PP NP
      NP -> Det NN | Det NN PP | NN | NP PP | 'CD' | NP VP
      VP -> Verb NP | VP PP
      Det -> 'DT'
      NN -> 'NN'
      Verb -> 'VB'
      PP -> 'PP' | IN NP
      IN -> 'IN'
      TO -> 'TO'
      SentenceEnd -> '.' | ':'
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

def tokenize_sentence(sentence: str):
    return nltk.word_tokenize(sentence)

def normalize_spacy_pos(pos: str, text: str):
    spacy_names = {
        'NN': 'NOUN',
        'NNS': 'NOUN',
        'NNP': 'NOUN',
        'JJ': 'ADJ',
        'RB': 'ADV',
        'HYPH': 'PUNCT',
        'DT': 'DET',
        'WDT': 'DET',
        ':': 'PUNCT',
        ',': 'PUNCT',
        'XX': 'PUNCT',
        '-LRB-': 'PUNCT',
        '-RRB-': 'PUNCT',
        'CD': 'NUM',
        'PRN': 'PRON',
        'PRP': 'PRON',
        'TO': 'PART',
        'POS': 'PART',
        'VB': 'VERB',
        'VBP': 'VERB',
        'VBN': 'VERB',
        'VBD': 'VERB',
        'VBG': 'VERB',
        'CC': 'CCONJ',
        'VBZ': 'AUX',
    }

    if pos == "IN":
       if text in ["if", "without"]:
           return "SCONJ"
       else:
           return "ADP"

    if text == "'re":
        return "AUX"

    if pos in spacy_names:
        return spacy_names[pos]
    
    return pos

def combine_punc_spacy(tokens: list[tuple[str,str]]):
    ndx = 0
    while (ndx < len(tokens)):
        current = tokens[ndx]
        if ndx < len(tokens)-2:
            tt = tokens[ndx+1]
            if tt[0] == "-":
                second = tokens[ndx+2]
                yield (current[0]+tt[0]+second[0],current[1])
                ndx = ndx + 2
            else:
                yield current
        else:
            yield current
        ndx = ndx +1

def combine_punc_stanza(tokens: list[any]):
    ndx = 0
    while (ndx < len(tokens)):
        current = tokens[ndx]
        if ndx < len(tokens)-2 and tokens[ndx+1].text == "-":
            tt = tokens[ndx+1]
            second = tokens[ndx+2]
            yield type('obj', (object,), {'text' : current.text+tt.text+second.text, 'upos': current.upos})
            #yield (current.text+tt.text+second.text,current.upos)
            ndx = ndx + 2
        elif ndx < len(tokens)-1 and (current.text.endswith("-") or tokens[ndx+1].text.startswith("-")):
            tt = tokens[ndx+1]
            yield type('obj', (object,), {'text' : current.text+tt.text, 'upos': current.upos})
            ndx = ndx + 1
        elif ndx < len(tokens)-1 and current.upos == "NUM" and tokens[ndx+1].upos == "NUM":
            tt = tokens[ndx+1]
            yield type('obj', (object,), {'text' : current.text+tt.text, 'upos': current.upos})
            ndx = ndx + 1
        else:
            yield current
        ndx = ndx +1

def analyse_sentence_spacy(sentence: str):
    #print('Processing using Spacy')
    doc = nlp(sentence)
    #print ([(token.text, token.pos_) for token in doc])
    assert doc.has_annotation("SENT_START")
    tagged_tokens: list[tuple[str, str]] = []
    for sent in doc.sents:
        #print(sent.text)
        if print_sentence_tree:
            nltk_spacy_tree(sent).pretty_print()
        if (str(sent) != "\n"):
            sent_tokens = [(token.text, token.tag_) for token in sent]
            tagged_tokens.append(sent_tokens)

    # print tagged tokens
    if print_tagged_tokens:
        #if len(tagged_tokens) > 1:
        #    print('[')
        for l in tagged_tokens:
            #if len(tagged_tokens) > 1:
            #    print('   ', l)
            #else:
                for token_sent in tagged_tokens:
                    for tt in combine_punc_spacy(token_sent):
                        if tt[0] != "\n" and tt[1] != "_SP":
                            pos_name = normalize_spacy_pos(tt[1], tt[0])
                            print('(', pos_name, ' ', tt[0], ')', sep="", end=" ")
                print()
        #if len(tagged_tokens) > 1:
        #    print(']')

    if detect_grammar:
        try:
            print('[')
            for l in tagged_tokens:
                print('   ', l)
                pos_tags = [normalize_token(token, pos) for (token,pos) in l]
                detected_sentences = 0
                for sentence_structure in grammar_parser.parse( filter(lambda x: x != '_SP', pos_tags) ):
                    print(sentence_structure)
                    detected_sentences += 1

                if (detected_sentences == 0):
                    print("Cannot detect sentence", pos_tags)
            print(']')
        except:
            print("Cannot detect sentence", pos_tags)

    if print_nltk_tokens:
        dump_tokens_nltk(tagged_tokens)

def normalize_token(token: str, pos: str):
    if (pos == '_SP'):
        return pos
    return pos[:2]

def dump_tokens_nltk(tagged_tokens: list[list[tuple[str,str]]]):
    pos_tags = [[normalize_token(token, pos) for (token,pos) in sent] for sent in tagged_tokens]
    #pos_tags = sum(pos_tags,[])
    print(list(set(sum(pos_tags,[]) )))

def analyse_sentence_nltk(sentence):
    print('Processing using NLTK')
    tokens = [tokenize_sentence(t) for t in nltk.sent_tokenize(sentence)]

    # parts of speech tagging
    tagged_tokens = nltk.pos_tag_sents(tokens)

    # print tagged tokens
    print('[')
    for l in tagged_tokens:
        print('   ', l)
    print(']')

    dump_tokens_nltk(tagged_tokens)

def analyse_sentence_stanza(sentence: str):
    doc = stanza(sentence)
    for i, sent in enumerate(doc.sentences):
        # print(f'====== Sentence {i+1} tokens =======')
        print(*[f'({token.upos} {token.text})' for token in combine_punc_stanza(sent.words)], sep=' ')


def analyse_sentence(sentence):
    print(sentence, end="")
    if process_nltk:
        analyse_sentence_nltk(sentence)
    if process_spacy:
        analyse_sentence_spacy(sentence)
    if process_stanza:
        analyse_sentence_stanza(sentence)

    print()

def sample():
    # input text
    sentence1 = """To Calculate factorial of a number: If a number is 0 then result is 1.
    If a number is 1 then result is 1.
    Let a previous number is a number minus 1.
    Calculate factorial of a previous number into a previous factorial.
    Result is a previous factorial multiply a number."""
    #analyse_sentence(sentence1)

    sentence2 = """let a value equals 10. Add 20 to a value. multiply a value by 42."""
    analyse_sentence(sentence2)

    sentence2 = """add 20 to a value."""
    analyse_sentence(sentence2)

    sentence2 = """multiply a value by 42."""
    analyse_sentence(sentence2)

    sentence3 = """To get answer to all questions: result is 42."""
    analyse_sentence(sentence3)

    sentence4 = """To calculate rectangle's area from a width and a height: multiply a width by a height."""
    #analyse_sentence(sentence4)

    sentence5 = """Subtract 1 from the substring's last."""
    analyse_sentence(sentence5)

    sentence6 = """Handle capitalize given the source's text."""
    analyse_sentence(sentence6)

    sentence7 = """Clear the source's text's last operation."""
    analyse_sentence(sentence7)

    sentence8 = """Input the number N whose square you want to find."""
    analyse_sentence(sentence8)

    sentence9 = """Store the result of the multiplication in a variable."""
    analyse_sentence(sentence9)

    sentence10 = """Output the value of the variable."""
    analyse_sentence(sentence10)

    analyse_sentence("""set the nectar of all flowers to 0.""") # https://www.cs.cmu.edu/~NatProg/papers/Myers2004NaturalProgramming.pdf
    analyse_sentence("""Define the bus state and initial state.""") # https://ntrs.nasa.gov/api/citations/20140004055/downloads/20140004055.pdf
    analyse_sentence("""Import low-level requirements for a bus.""") # https://ntrs.nasa.gov/api/citations/20140004055/downloads/20140004055.pdf
    analyse_sentence("""Compute next state of Bus and Right Side.""") # https://ntrs.nasa.gov/api/citations/20140004055/downloads/20140004055.pdf

    analyse_sentence("""Import goods from China.""")
    analyse_sentence("""Construct a house.""")
    analyse_sentence("""Destroy his confidence.""")
    analyse_sentence("""Observe the reaction.""")
    analyse_sentence("""Peruse the index.""")
    analyse_sentence("""Speak to me.""")
    analyse_sentence("""Prove that every reachable state is a valid state.""") # https://ntrs.nasa.gov/api/citations/20140004055/downloads/20140004055.pdf
    analyse_sentence("""Counts the lines of text in a folder full of text files.""")
    analyse_sentence("""Say yes.""")
    analyse_sentence("""Beep three times.""")
    analyse_sentence("""Move that email to the trash folder.""")
    analyse_sentence("""Return 1 if number is smaller 2.""")
    analyse_sentence("""Assert fibonacci of 5 is 8.""")
    analyse_sentence("""To toggle the light in a room.""")
    analyse_sentence("""Integrate a function from x to y.""")

    analyse_sentence("""Increment the turn count.""") # https://ganelson.github.io/inform-website/book/WI_19_15.html#e121
    analyse_sentence("""Increase the time of day by 5 minutes.""")
    analyse_sentence("""Follow the time allotment rules.""")
    analyse_sentence("""Continue the action.""")
    analyse_sentence("""decrease the price of the money by the price of the noun.""") #https://ganelson.github.io/inform-website/book/WI_15_19.html#e66
    analyse_sentence("""Schedule launch with an owner at 12.""") # https://www.cs.cmu.edu/~jgc/Student%20Dissertations/1989-Jill%20Fain%20Lehman.pdf
    analyse_sentence("""Cancel my 3 o'clock appointment.""") # https://www.cs.cmu.edu/~jgc/Student%20Dissertations/1989-Jill%20Fain%20Lehman.pdf
    analyse_sentence("""Delete the meeting beginning at 3:00 p.m""") # https://www.cs.cmu.edu/~jgc/Student%20Dissertations/1989-Jill%20Fain%20Lehman.pdf
    analyse_sentence("""Display flight information for flights from New York to Pittsburgh.""") # https://www.cs.cmu.edu/~jgc/Student%20Dissertations/1989-Jill%20Fain%20Lehman.pdf

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
    [('To', 'TO'), ('get', 'VB  '), ('answer', 'JJR'), ('to', 'TO'), ('all', 'DT'), ('questions', 'NNS'), (':', ':'), ('result', 'NN'), ('is', 'VBZ'), ('42', 'CD'), ('.', '.')]
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

def analyze_file(filename: str, lines: bool):
    with open(filename, "r+", encoding="utf-8") as program_file:
        if lines:
            # Reading from a file
            for program_content in program_file.readlines(): 
                analyse_sentence(program_content)
        else:
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
arg_parser.add_argument('--lines',
                    action='store_true') 
arg_parser.add_argument('-v', '--verbose',
                    action='store_true')  # on/off flag

args = arg_parser.parse_args()
print(args.command, args.filename)

match args.command:
    case "sample":
        sample()
    case "analyze":
        filename = args.filename
        lines = args.lines
        analyze_file(filename, lines)
    case "sentencize":
        filename = args.filename
        output_file = args.output if args.output else args.filename +'.sentence'
        split_sentence_spacy(filename, output_file)
