#using <mscorlib.dll>
#using "PreProcessor.netmodule"

using namespace System;

public __gc class HelloWorldC
{
    public:
        // Provide .NET interop and garbage collecting to the pointer.
        Class1 __gc *t;
        HelloWorldC() {
            t = new Class1();
            // Assign the reference a new instance of the object
        }
        
     // This inline function is called from the C++ Code
        void callCSharpHelloWorld() {
            t->displayMessage();
        }
};