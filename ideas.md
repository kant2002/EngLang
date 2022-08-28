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
    if recognition Result Successful then Do nothing.

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