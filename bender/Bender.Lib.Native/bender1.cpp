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


double TwoDRead(double* arr, int i, int j, int ny)
{
	return arr[i * ny + j];
}

double TwoDRead(uint16_t* arr, int i, int j, int ny)
{
	return arr[i * ny + j];
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
		double residAbsSum = 0;
		int numResid = 0;
		for (int i = 1; i < nx - 1; i++)
		{
			for (int j = 1; j < ny - 1; j++)
			{
				if (id[i, j] == 0)
				{
					double neighborMean = 0.25 * (v[i - 1, j] + v[i + 1, j] + v[i, j - 1] + v[i, j + 1]);
					double residual = v[i, j] - neighborMean;

					residAbsSum += abs(residual);
					numResid++;
					v[i, j] -= relaxationFactor * residual;
				}
			}
		}






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


