EngLang
=======

Interpreter for programming language in English.

Goal of this project is explore typesystem and grammar which allow write programs in English language.
Programs should be able to integrate with existing software development ecosystem, for example by accessing functions from dynamic libraries available on the system (.dll or .so).

Would be good to have this language compile not only to executables, but to dlls too. Which in itself can be used from other programming languages.

I will target initially .NET runtime, since I know it relatively well.
Other goal is to be able translate existing languages to this language. This is to support explanability of codebase to non-programmer.

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
resolve an domain name =>
   call .NET Method `Dns.GetHostAddresses` with domain name as parameter and obtain the addresses
```

it would be equivalent to following pseudo code.
```
resolve(domain_name):
    addresses = Dns.GetHostAddresses(domain_name)
```

Self-contained function which calculated Fibonacci number
```
calulate factorial of a number =>
    if the number is 0 then return 1
    if the number is 1 then return 1
    let previous number is number minus 1
    calculate factorial of the previous number into the previous factorial
    return the previous factorial multiply the number
```
After looking at this sample it is not clear to me how 
- take reference results of function execution.
- how define calculations into new variables. That's much easier to get right, but better be careful.
- how to return value from function.
- Is functions are valid constructs for humans?

# Links
- https://www.cs.cmu.edu/~jgc/Student%20Dissertations/1989-Jill%20Fain%20Lehman.pdf
- https://github.com/pannous/english-script
- http://inform7.com/doc/
- https://en.wikipedia.org/wiki/Literate_programming