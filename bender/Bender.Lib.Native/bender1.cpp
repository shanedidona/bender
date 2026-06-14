#include <cstdint>
#include <vector>
#ifdef _WIN32
	#define EXPORT extern "C" __declspec(dllexport)
#else
	#define EXPORT extern "C"
#endif

EXPORT int Add(int a, int b)
{
	return a + b;
}

EXPORT int Solve1(
	double* v,
	const uint16_t* id,
	int nx,
	int ny,
	double relaxationFactor,
	double meanAbsChangeStop,
	int maxTries,
	double** outMeanAbsChangeArray,
	int* outMeanAbsChangeArrayLen
)
{
	std::vector<double> woooo;
	woooo.push_back(2.3);








	int outArrayLen = woooo.size();
	double* outArray = new double[outArrayLen];
	std::copy(woooo.begin(), woooo.end(), outArray);
	*outMeanAbsChangeArray = outArray;
	*outMeanAbsChangeArrayLen = outArrayLen;

	return 1;
}

EXPORT void DeleteDoubleArray(double* pointer)
{
	delete[] pointer;
}


