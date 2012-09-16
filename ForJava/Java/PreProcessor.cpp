#include <jni.h>
#include "C:\Users\VARUN\Documents\GitHub\project-pinnacle\ForJava\Java\PreProcessorJava.h"

// The Managed C++ header containing the call to the C#
#include "C:\Users\VARUN\Documents\GitHub\project-pinnacle\ForJava\MCPP\PreProcessor.h"
#include <string>
using namespace std;

// This is the JNI call to the Managed C++ Class
// NOTE: When the java header was created, the package name was not include in the JNI call.
// Java_<package name>_<class name>_<method name>

void GetJStringContent(JNIEnv *AEnv, jstring AStr, std::string &ARes) {
  if (!AStr) {
    ARes.clear();
    return;
  }

  const char *s = AEnv->GetStringUTFChars(AStr,NULL);
  ARes=s;
  AEnv->ReleaseStringUTFChars(AStr,s);
}

JNIEXPORT void JNICALL Java_Test1_PreProcess (JNIEnv * env, jobject jobj, jstring s1, jstring s2, jint b, jint c){

	std::string str1;
	std::string str2;
	GetJStringContent(env,s1,str1);
	GetJStringContent(env,s2,str2);

    // Instantiate the MC++ class.
    PreProcessorC* t = new PreProcessorC();

    // The actual call is made. 
    t->callCSharpPreProcess(str1,str2,b,c);
}