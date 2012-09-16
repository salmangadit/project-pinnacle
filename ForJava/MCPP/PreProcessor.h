#using <mscorlib.dll>
#using "PreProcessor.netmodule"
#include <string>

using namespace std;
using namespace System;

public __gc class PreProcessorC
{
    public:
        // Provide .NET interop and garbage collecting to the pointer.
        PreProcessor __gc *t;
        PreProcessorC() {
            t = new PreProcessor();
            // Assign the reference a new instance of the object
        }
        
     // This inline function is called from the C++ Code
        void callCSharpPreProcess(string s1,string s2, int b, int c) {
			System::String* str1 = s1.c_str();
			System::String* str2 = s2.c_str();
            t->PreProcess(str1,str2,b,c);
        }
};