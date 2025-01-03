EngLang
=======

Interpreter for programming language in English.

Goal of this project is explore typesystem and grammar which allow write programs in English language.
Programs should be able to integrate with existing software development ecosystem, for example by accessing functions from dynamic libraries available on the system (.dll or .so).

Would be good to have this language compile not only to executables, but to dlls too. Which in itself can be used from other programming languages.

I will target initially .NET runtime, since I know it relatively well.
Other goal is to be able translate existing languages to this language. This is to support ability to explain of codebase to non-programmer.

I would choose English as example, since it has nice properties like articles. This allow disambiguate types from variables. I do not know what reasonable language trick [allow that](https://ru.wikipedia.org/wiki/%D0%90%D1%80%D1%82%D0%B8%D0%BA%D0%BB%D1%8C#%D0%A2%D0%B8%D0%BF%D1%8B_%D0%B0%D1%80%D1%82%D0%B8%D0%BA%D0%BB%D0%B5%D0%B9) in Russian/Ukrainian for example. If somebody know that I may pursue that language too.

Problems which I see with this kind of languages, is that software developers would oppose adoption of such language because it would not serve any practical purposes for engineering. I would say that any practical purposes would be in education maybe in understanding of the existing systems, gluing of the exising code (a.k.a no-code).
Reasons for that - most users cannot handle rigor required for automation. Reasons in Russian below: 
![Algorithm](./2_fortran-1.jpg "Algorithm and what it is")
![Algorithm](./3_fortran.jpg "How hard to write algorithm for robot")

Other problem which I see is embedding of the well known data structures or formats in the language. This is closely resemble following samples when communicate with API for example. For example, how I would like to describe making API call to server in English?

Multiple domains can reuse same words, but with slightly specific meaning. Programming language should allow same names used within different context. Ideally you can mix concepts from different contexts.

## Ideas

How to declare variable: `an apple`. This is implicitly create variable `apple`. Let's say this is declaration of the variable. How to reference variable: `the apple`.

Interop with .NET
```
resolve a domain name ->
   call .NET Method `Dns.GetHostAddresses` with a domain name as parameter into the addresses.
```

it would be equivalent to following pseudo code.
```
resolve(domain_name):
    addresses = Dns.GetHostAddresses(domain_name)
```

Self-contained function which calculated Fibonacci number
```
Calculate factorial of a number ->
    if a number is 0 then result is 1.
    if a number is 1 then result is 1.
    let a previous number is a number minus 1.
    calculate factorial of a previous number into a previous factorial.
    result is a previous factorial multiply a number.
```

Maybe something more pythonnish
```
define the factorial of a number as
    if number is 0 then result is 1.
    result is a number times factorial of a number minus one.

let a number is factorial of 5
```
or 
```
define the factorial of a number as
    if number is 0 then result is 1.
    let a previous factorial be factorial of number minus one.
    result is a number times a previous factorial.

let a number is factorial of 5
```
or
```

to calculate the fibonacci of a number ->
   result is 1 if number is smaller 2.
   let a last be fibonacci of a number minus 1.
   let a one before last be fibonacci of a number minus 2.
   result is a last plus a one before last.
end
```

After looking at this sample it is not clear to me how 
- take reference to results of function execution.
- how define calculations into new variables. That's much easier to get right, but better be careful.
- Is functions are valid constructs for humans?

Implicit return of last expression (eventually, I cannot build linguistic construct which looks natural).
```
the width is a number.
the height is a number.

To calculate area from a width and a height ->
  result is a width multiplied by a height.
```

Do language support concept of an expression in itself? How we decide that specific construct is expression, and what is statement.

### Determiners

Maybe I can use determiners like articles for English for variable specification.
What can I do for determiner-less languages like Russian(?). Have to research this article [5]

Alternative determiners looks promising.

## Sentences

The statements is sentence.

### Simple sentences

Statement is a sentence which ends with `.`. Long sentences can be split by `;`, each part of this sentence will become their own sentence.

```
statement_1 ; statement_2 ; .... ; statement_N.
```

Preferably operate using linguistic properties, and do not use `;` if possible since people nowadays don't use it very much.

### Labeled sentences

Functions is labeled list of statements. 
```
Label_sentence_L1 ->
Label_sentence_L2 ->
  statement_1.
  statement_2.
  .....
  statement_3.
```

This label in itself will declare parameters, and specify how to execute call. The multiple labels can point to same set of sentences. That's allow for aliases for functions.

### Conditional execution

Conditional statement supports only constructs which can be considered `if..then` in other languages.

```
if XXXX is YYYY, operations_list.
```

Classical `if..then` is also an option.

```
if XXXX is YYYY then operations_list.
```

Maybe `then` can be used for chaining operations

```
add 20 to a value then multiply a value by 42.
```

Upon thinking about it more. Let's pretend that `if` executes single function, and then executes another statement if result is true/positive.
That allow generalize constructions like `XXX is YYY` to any form of invocation of expression.

### Arithmetic

It seems to be that math operations carried in usual language is quite interesting.

```
let a value equals 10.
add 20 to a value.
multiply a value by 42.
```

Can be roughly translated to
```
let value = 10;
value += 20;
value *= 42;
```

This is not best way to write mathematical expression, but best way to validate calculations by humans without special training.

Assignment of variable can happens using `set`/`put`.
```
set a value to 10.
```
or
```
put 10 into a value.
```

### String operations

```
append "value" to an error message
```

also we can chain string values together using `then` operator.
```
puth "value" then "another value" into a message.
```

which will place two string one after another into a message variable.

### Accessing properties

When speaking about object properties we usually use `of`
```
the size of the box
the constraints of the parent container
width of the content
the color of the textual
```

or  `'s`

```
paragraph's text.
the element's color.
an element's text.
```

so in a sense that allow us to view objects as bag of properties. Let's make this as hypothesis.

### Classes

Classes is definition for objects.

```
an apple is an fruit
```

or classical shapes

```
a circle is a shape with
    a radius.

a rectangle is a shape with
    a width
    and a height.
```

We can have aliases for the shape slots. Similar to C unions.

```
a pen has
    a color,
    a width,
    a size at the width.
```

In case we need to have two properties of same type, let's name them.

```
a pen has
    a color,
    a width,
    a number named width.
```

## Arrays or lists

I think we as humans operate on some objects. Language assume that we either have some set of ordered objects. Probably closer to dynamic lists.
Arrays is more imporants.

```
the apples are some fruits.
```

or

```
the apples are fruits.
```

assuming that `fruit` is some shape. Pluralization is important for humans.

```
the apples are some fruit.
```

look really wierd.

# Links
- [1] https://www.cs.cmu.edu/~jgc/Student%20Dissertations/1989-Jill%20Fain%20Lehman.pdf
- [2] https://github.com/pannous/english-script
- [3] http://inform7.com/doc/
- [4] https://en.wikipedia.org/wiki/Literate_programming
- [5] https://tspace.library.utoronto.ca/bitstream/1807/26223/1/Piriyawiboon_Nattaya_201011_PhD_thesis.pdf
- [6] https://hyperscript.org/reference/
- [7] http://attempto.ifi.uzh.ch/site/
- [8] https://developer.apple.com/library/archive/documentation/AppleScript/Conceptual/AppleScriptLangGuide/introduction/ASLR_intro.html
- [9] https://web.archive.org/web/20060315150220/http://frontier.userland.com/manual/objectDatabase
- [10] https://github.com/hhas/iris-script/wiki
- [11] https://wwwcdn.imo.org/localresources/en/OurWork/Safety/Documents/A.918(22).pdf
- [12] https://simple.wikipedia.org/wiki/Basic_English
- [13] https://ntrs.nasa.gov/api/citations/20140004055/downloads/20140004055.pdf
- [14] https://osmosianplainenglishprogramming.blog/
- [15] https://web.media.mit.edu/%7Elieber/Publications/Programmatic-Semantics.pdf
- [15] https://web.media.mit.edu/%7Elieber/Publications/Feasibility-Nat-Lang-Prog.pdf
- [16] https://www.cs.cmu.edu/~NatProg/papers/Myers2004NaturalProgramming.pdf
- [17] https://en.wikipedia.org/wiki/Definite_clause_grammar
- [16] https://aclanthology.org/J88-4001.pdf
- [17] https://link.springer.com/article/10.1007/s10462-015-9449-3
- [18] http://www.bitsavers.org/pdf/univac/flow-matic/U1518_FLOW-MATIC_Programming_System_1958.pdf
