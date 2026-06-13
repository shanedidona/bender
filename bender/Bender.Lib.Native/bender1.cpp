#ifdef _WIN32
	#define EXPORT extern "C" __declspec(dllexport)
#else
	#define EXPORT extern "C"
#endif

EXPORT int Add(int a, int b)
{
	return a + b;
}




