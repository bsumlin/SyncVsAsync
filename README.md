# SyncVsAsync
## A very simple C# UWP Async Example
This stupidly simple app simulates a demanding UI and shows what happens when you do synchronous and asynchronous procedures. The code is documented, but feel free to e-mail me with questions.

This app calculates the nth prime number. In the top text box, input n. Then click either "Do Synchronous Operation" or "Do Asynchronous Operation". Those buttons should be self-explanatory. Notice what happens to the UI by monitoring the clock while executing either function.

This is not meant to be an exhaustive summary of sync vs async. I wrote this up more for practice than for education. I do a lot of math in my actual apps and I needed to learn how to do it correctly without blocking a computationally-expensive UI (lots of graphs). I'm not even guaranteeing this is best-practices for async programming. Hopefully it helps someone, though.
