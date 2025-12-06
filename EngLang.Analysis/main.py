import argparse

from analysis import analyze_file, analyze_sample, split_sentence_spacy

def main():
    arg_parser = argparse.ArgumentParser(
        prog = 'ProgramName',
        description = 'What the program does',
        epilog = 'Text at the bottom of help')
    arg_parser.add_argument('command', nargs='?', default = 'sample')
    arg_parser.add_argument('-f', '--filename')
    arg_parser.add_argument('-d', '--directory')
    #arg_parser.add_argument('-r', '--recursive')
    arg_parser.add_argument('-o', '--output', required = False)
    arg_parser.add_argument('--lines',
                        action='store_true') 
    arg_parser.add_argument('-v', '--verbose',
                        action='store_true')  # on/off flag

    args = arg_parser.parse_args()
    print(args.command, args.filename)

    match args.command:
        case "sample":
            analyze_sample()
        case "analyze":
            filename = args.filename
            lines = args.lines
            directory = args.directory
            if (filename):
                analyze_file(filename, lines)
            elif (directory):
                import os
                for root, dirs, files in os.walk(directory):
                    for file in files:
                        if file.endswith('.lines'):
                            full_path = os.path.join(root, file)
                            print("Analyzing file:", full_path)
                            analyze_file(full_path, lines)
            else:
                print("Please provide a filename or directory to analyze.")
        case "sentencize":
            filename = args.filename
            output_file = args.output if args.output else args.filename +'.sentence'
            split_sentence_spacy(filename, output_file)



if __name__ == "__main__":
    main()
