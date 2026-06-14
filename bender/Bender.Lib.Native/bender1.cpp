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

uint16_t TwoDRead(const uint16_t* arr, int i, int j, int ny)
{
	return arr[i * ny + j];
}

void TwoDModify(double* arr, int i, int j, int ny, double delta)
{
	arr[i * ny + j] += delta;
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

		//Middle
		for (int i = 1; i < nx - 1; i++)
		{
			for (int j = 1; j < ny - 1; j++)
			{
				if (TwoDRead(id, i, j, ny) == 0)
				{
					double neighborMean = 0.25 *
						(
							TwoDRead(v, i - 1, j, ny)
							+ TwoDRead(v, i + 1, j, ny)
							+ TwoDRead(v, i, j - 1, ny)
							+ TwoDRead(v, i, j + 1, ny)
							);
					double residual = TwoDRead(v, i, j, ny) - neighborMean;

					residAbsSum += abs(residual);
					numResid++;
					TwoDModify(v, i, j, ny, -relaxationFactor * residual);
				}
			}
		}

		//i==0 Edge (but not corners)
		for (int j = 1; j < ny - 1; j++)
		{
			int i = 0;
			if (TwoDRead(id, i, j, ny) == 0)
			{
				double neighborMean = oneOver3 *
					(
						TwoDRead(v, i + 1, j, ny)
						+ TwoDRead(v, i, j - 1, ny)
						+ TwoDRead(v, i, j + 1, ny)
						);
				double residual = TwoDRead(v, i, j, ny) - neighborMean;

				residAbsSum += abs(residual);
				numResid++;
				TwoDModify(v, i, j, ny, -relaxationFactor * residual);
			}
		}

		//i==0 Edge (but not corners)
		for (int j = 1; j < ny - 1; j++)
		{
			int i = 0;
			if (TwoDRead(id, i, j, ny) == 0)
			{
				double neighborMean = oneOver3 *
					(
						TwoDRead(v, i + 1, j, ny)
						+ TwoDRead(v, i, j - 1, ny)
						+ TwoDRead(v, i, j + 1, ny)
						);
				double residual = TwoDRead(v, i, j, ny) - neighborMean;

				residAbsSum += abs(residual);
				numResid++;
				TwoDModify(v, i, j, ny, -relaxationFactor * residual);
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


