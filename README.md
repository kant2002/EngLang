EngLang
=======

Interpreter for programming language in English.

Goal of this project is explore typesystem and grammar which allow write programs.
Programs should be able to integrate with existing ecosystem, for example by accessing functions from dynamic libraries available on the system (.dll or .so).

Would be good to have this language compile not only to executables, but to dlls too. Which in itself can be used from other programming languages.

I will target initially .NET runtime, since I know it relatively well.
Other goal is to be able translate existing languages to this language. This is to support explanability of codebase to non-programmer.

I would choose English as example, since it has nice properties like articles. This allow disambiguate types from variables. I do not know what reasonable language trick [allow that](https://ru.wikipedia.org/wiki/%D0%90%D1%80%D1%82%D0%B8%D0%BA%D0%BB%D1%8C#%D0%A2%D0%B8%D0%BF%D1%8B_%D0%B0%D1%80%D1%82%D0%B8%D0%BA%D0%BB%D0%B5%D0%B9) in Russian/Ukrainian for example. If somebody know that I may pursue that language too.

Problems which I see with this kind of languages, is that software developers would oppose adoption of such language because it would not serve any practical purposes for engineering. I would say that any practical purposes would be in education maybe in understanding of the existing systems, gluing of the exising code (a.k.a no-code).
Reasons for that - most users cannot handle rigor required for automation.
![Algorithm](/2_fortran-1.jpg "Algorithm and what it is")
![Algorithm](/3_fortran.jpg "How hard to write algorithm for robot")

Other problem which I see is embedding of the well known data structures or formats in the language. This is closely resemble following samples when communicate with API for example. For example, how I would like to describe making API call to server in English?

# Links
- https://www.cs.cmu.edu/~jgc/Student%20Dissertations/1989-Jill%20Fain%20Lehman.pdf
- https://github.com/pannous/english-script
- http://inform7.com/doc/