My ideas
========

This file is place for some wild ideas to capture. Even maybe I would not comeback to them, process of capturing ideas hopefully gives me a way to structure my understanding of the problem.

# Process class as context

Let's say we have a class in C#/JS which sitting somewhere in the business layer. Given modern state of enterprise development, 
this class instantiated using dependency injection, so he receives all his fields in the constructor. Then operations/methods 
of this class just perform some operations on these dependencies, and mostly do not do anything special.

For example consider this situation. We have barrier solution which allow car pass through single barrier.

```csharp
enum Decision
{
    DoNothing,
    DenyEntry,
    AllowEntry,
}

class SecurityGuard
{
    private IBarrier barrier;
    private ILicensePlateRecognitionSystem plateRecognition;
    private ILicensePlateJournal journal;

    public SecurityGuard(IBarrier barrier, ILicensePlateRecognitionSystem plateRecognition, ILicensePlateJournal journal)
    {
        this.barrier = barrier;
        this.plateRecognition = plateRecognition;
        this.journal = journal;
    }

    public Decision InspectCarBeforeBarrier()
    {
        var recognitionResult = plateRecognition.RecogniseLicensePlate();
        if (!recognitionResult.Success)
        {
            return Decision.DoNothing;
        }

        var licensePlate = recognitionResult.LicensePlate;
        var licensePlateKnown = journal.IsLicensePlateKnown(licensePlate);
        if (licensePlateKnown)
        {
            journal.RecordCarAttendance(Decision.AllowEntry, licensePlate);
            barrier.OpenBarrier();
            return Decision.AllowEntry;
        }
        else
        {
            journal.RecordCarAttendance(Decision.DenyEntry, licensePlate);
            return DenyEntry;
        }
    }
}
```

Classically in teaching what class is, it represents as data and function on that data, but for these process-like classes data
is more like context for expressing operations for the role or process. I call these classes process-like because from my point of view,
they are just expressing business process of some sort. Barrier, License Plate Recognition system and Record Of Visits(Journal).
So when you wrote process description for security guard without automation, you probably in your document will implicitly reference
mentions of the barrier, license plate recognition system and journal where all recording of event will happens. That's still common 
in some parts of the world. License plate recognition system maybe as simple as CCTV cameras and security guy will "recognize" plate 
using his own eyes.

Human readable description in pseudo code.
```
To Inspect Car Before Barrier:
    Recognise License Plate.
    if recognition Result is not Successful then Do nothing.

    license Plate is license plate from recognition Result.
    license Plate Known if it is Is License Plate Registered in journal.
    if license Plate Known then
    {
        Record Car Attendance in journal as Allow Entry and license Plate.
        Open Barrier.
    }
    else
    {
        Record Car Attendance in journal as Deny Entry and license Plate.
    }
```

I want to distinguish between these process-like classes and regular classes/objects because objects operate on data, but these processes operate in the context. Technically objects which form context are still some data, but mostly these objects are some mechanism to perform action, so they clearly separate from regular data in the original sense.

# Embedding expression in text

Based on reading code from AppleScript and EnglishScript I came to conclusion that 
embedding simple mathy expressions is noble goal, and make language more practical. 
Reading formulas is already better for anybody who is somewhat technical. Reading 
same formulas in pure text is most likely inconvenience for everybody.

# Events

Based on reading code from AppleScript and EnglishScript I see events handling as first-class citizen.
Subscript to events are pretty easy to implement, but how to declare them?

# Linguistic underpinning

I would base my terms based on how Catalyst report linguistic tokens, not how they are linguistically correct.

Variable declaration.

```
the console is a console.
the red console is a console.
the my console is a console.
the red vibrating console is a console.
```

translates to 
```
DET(the) NOUN(console) VERB(is) DET(a) NOUN(console) PUNCT(.)
DET(the) ADJ(red) NOUN(console) VERB(is) DET(a) NOUN(console) PUNCT(.)
DET(the) DET(my) NOUN(console) VERB(is) DET(a) NOUN(console) PUNCT(.)
DET(the) ADJ(red) NOUN(vibrating) NOUN(console) VERB(is) DET(a) NOUN(console) PUNCT(.)
```

so let's say variable declaration is
```
DET(the) DET? ADJ+ NOUN+ VERB(is) DET(a|an) DET? ADJ+ NOUN+ PUNCT(.)
```

# Proximity

Maybe that would be not fruitful, but let's map stack and heap on concepts of proximity which humans operate on. 
Stack is objects which we are currently working on and sitting on your table with hands reach. 
You can easily manipulate them. Heap is on the other side some remote storage.

# In-place operations

I choose in-place arithmetic operations as part of calculations and that's starts biting. Where `divide a value by 42` 
is perfectly in-place operations, `multiply a width of a rectangle by a height of a rectangle` cannot be thought
like that. It's more regular operation. I thinking about that, like what I thought as in-place operation is actually
operation on hidden register with context dependent flushing. So we start operation, perform them, store result in the
memory cell, then can continue operation and so on. Now depending on the context, like if this regular statement we dump
result in original location, or in case if this sentence is last sentence in function we return value from that temporary
register.
