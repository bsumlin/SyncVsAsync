# SyncVsAsync
## A very simple C# UWP Async/Parallel programming example
This stupidly simple app simulates a demanding UI and shows what happens when you do synchronous and asynchronous procedures. The code is documented, but feel free to e-mail me with questions.

This app calculates the nth prime number. In the top text boxes, input n. Then click any of the buttons. Notice what happens to the UI by monitoring the clock while executing synchronous or asynchronous operations.

Also, you can execute the tasks in serial or parallel. After running a function, the app provides the execution time via the System.Diagnostics.Stopwatch class.

This is not meant to be an exhaustive summary of sync vs async or serial vs parallel. I wrote this up more for practice than for education. I do a lot of math in my actual apps and I needed to learn how to do it correctly without blocking a computationally-expensive UI (lots of graphs). I'm not even guaranteeing this is best-practices for async or parallel programming. Hopefully it helps someone, though.
