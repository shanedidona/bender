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
	double oneOver3 = 1.0 / 3.0;
	std::vector<double> out1Vector;

	for (int tryI = 0; tryI < maxTries; tryI++)
	{






	}















	
	out1Vector.push_back(2.3);








	int outArrayLen = out1Vector.size();
	double* outArray = new double[outArrayLen];
	std::copy(out1Vector.begin(), out1Vector.end(), outArray);
	*outMeanAbsChangeArray = outArray;
	*outMeanAbsChangeArrayLen = outArrayLen;

	return 1;
}

EXPORT void DeleteDoubleArray(double* pointer)
{
	delete[] pointer;
}


