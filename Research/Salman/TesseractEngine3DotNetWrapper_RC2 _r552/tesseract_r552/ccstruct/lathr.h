/**
Copyright 2011, Cong Nguyen

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

#ifndef TESSERACT_CCMAIN_LATHR_H__
#define TESSERACT_CCMAIN_LATHR_H__

#include "errcode.h"
#include <math.h>

namespace tesseract {

class LocalAdaptiveThreshold
{
public: /*Calc Integral*/
	static void CalcIntegral(const unsigned char* imagedata, 
		int bytes_per_pixel, int bytes_per_line, 
		int roiLeft, int roiTop, int roiWidth, int roiHeight, 
		double vNormalize, // this value makes sure that integral at each level is converged to 0
		double* integral)
	{
		/**
		What is the normalized value:
		- It is actually average/mean intensity of the region of interest (or whole image).
		- As we assume that all pixels are the same probability, the mean intensity
		will make sure that integral at each level is converged to 0.
		- What does the problem will occur if we have not normalized intensity? 
		=> In this case, the integral function always increase, so over-load error maybe occur.
		**/

		bool toGrey = (bytes_per_pixel == 3 || bytes_per_pixel == 4);
		int r = (toGrey ? (bytes_per_pixel - 1) : 0);
		int g = (toGrey ? (bytes_per_pixel - 2) : 0);
		int b = (toGrey ? (bytes_per_pixel - 3) : 0);

#define floatGrey(p) (( \
		*(p + r) * 29 + \
		*(p + g) * 150 + \
		*(p + b) * 77 + \
		128) / 256.0f)

		int image_width = bytes_per_line / bytes_per_pixel;
		
		// calc offset
		int image_offset = roiTop * bytes_per_line + roiLeft * bytes_per_pixel;
		int dst_offset = 0;
		
		// temporate variables
		int x, y, image_index, dst_index;
		int x_1y_1 = -roiWidth - 1;
		int xy_1 = -roiWidth;
		int x_1y = -1;
		
		const unsigned char* pSrc = imagedata;
		double* pDst = integral;

		switch (bytes_per_pixel)
		{
		case 1:			
		case 3:			
		case 4:			
			pDst[dst_offset] = 
				(toGrey ? floatGrey(pSrc + image_offset) : pSrc[image_offset]) - vNormalize;
			// first line
			dst_index = dst_offset + 1; 
			image_index = image_offset + bytes_per_pixel;
			for (x = 1; x < roiWidth ; x++, dst_index++, image_index += bytes_per_pixel)
			{
				// calc integral
				pDst[dst_index] = 
					pDst[dst_index - 1] + 
					((toGrey ? floatGrey(pSrc + image_index) : pSrc[image_index]) - vNormalize);
			}

			// first column
			dst_index = dst_offset + roiWidth; 
			image_index = (roiTop + 1) * bytes_per_line + (roiLeft * bytes_per_pixel);
			for (y = 1; y < roiHeight; y++, dst_index += roiWidth, image_index += bytes_per_line)
			{
				// calc integral
				pDst[dst_index] = 
					pDst[dst_index - roiWidth] + 
					((toGrey ? floatGrey(pSrc + image_index) : pSrc[image_index]) - vNormalize);
			}

			// remains
			for (y = 1; y < roiHeight; y++)
			{
				image_index = (roiTop + y) * bytes_per_line + (roiLeft + 1)*bytes_per_pixel;
				dst_index = y * roiWidth + 1;
				for (x = 1; x < roiWidth; x++, image_index += bytes_per_pixel, dst_index++)
				{
					// calc integral
					pDst[dst_index] =
						pDst[dst_index + x_1y] + 
						pDst[dst_index + xy_1] -
						pDst[dst_index + x_1y_1] + 
						((toGrey ? floatGrey(pSrc + image_index) : pSrc[image_index]) - vNormalize);
				}
			}
			break;
		}
	}

	static void CalcIntegral(const unsigned char* imagedata, 
	int bytes_per_pixel, int bytes_per_line, 
	int roiLeft, int roiTop, int roiWidth, int roiHeight, 
	double vNormalize, // this value makes sure that integral at each level is converged to 0
	double* integral,
	double* varIntegral)
	{
		/**
		What is the normalized value:
		- It is actually average/mean intensity of the region of interest (or whole image).
		- As we assume that all pixels are the same probability, the mean intensity
		will make sure that integral at each level is converged to 0.
		- What does the problem will occur if we have not normalized intensity? 
		=> In this case, the integral function always increase, so over-load error maybe occur.
		**/

		bool toGrey = (bytes_per_pixel == 3 || bytes_per_pixel == 4);
		int r = (toGrey ? (bytes_per_pixel - 1) : 0);
		int g = (toGrey ? (bytes_per_pixel - 2) : 0);
		int b = (toGrey ? (bytes_per_pixel - 3) : 0);

#define floatGrey(p) (( \
		*(p + r) * 29 + \
		*(p + g) * 150 + \
		*(p + b) * 77 + \
		128) / 256.0f)

		int image_width = bytes_per_line / bytes_per_pixel;
		
		// calc offset
		int image_offset = roiTop * bytes_per_line + roiLeft * bytes_per_pixel;
		int dst_offset = 0;
		
		// temporate variables
		int x, y, image_index, dst_index;
		int x_1y_1 = -roiWidth - 1;
		int xy_1 = -roiWidth;
		int x_1y = -1;

		float intensity;
		double sqrV = vNormalize * vNormalize;
		
		const unsigned char* pSrc = imagedata;
		double* pDst = integral;
		double* pVar = varIntegral;

		switch (bytes_per_pixel)
		{
		case 1:			
		case 3:			
		case 4:
			// calc intensity
			intensity = 
				(toGrey ? floatGrey(pSrc + image_offset) : pSrc[image_offset]);
			// calc integral
			pDst[dst_offset] = intensity - vNormalize;
			pVar[dst_offset] = intensity * intensity - sqrV;

			// first line
			dst_index = dst_offset + 1; 
			image_index = image_offset + bytes_per_pixel;
			for (x = 1; x < roiWidth ; x++, dst_index++, image_index += bytes_per_pixel)
			{
				// calc intensity
				intensity = 
					(toGrey ? floatGrey(pSrc + image_offset) : pSrc[image_offset]);

				// calc integral
				pDst[dst_index] = 
					pDst[dst_index - 1] + (intensity - vNormalize);
				pVar[dst_index] = 
					pVar[dst_index - 1] + intensity * intensity - sqrV;
			}

			// first column
			dst_index = dst_offset + roiWidth; 
			image_index = (roiTop + 1) * bytes_per_line + (roiLeft * bytes_per_pixel);
			for (y = 1; y < roiHeight; y++, dst_index += roiWidth, image_index += bytes_per_line)
			{
				// calc intensity
				intensity = 
					(toGrey ? floatGrey(pSrc + image_offset) : pSrc[image_offset]);

				// calc integral
				pDst[dst_index] = 
					pDst[dst_index - roiWidth] + (intensity - vNormalize);
				pVar[dst_index] =
					pVar[dst_index - roiWidth] + (intensity * intensity - sqrV);
			}

			// remains
			for (y = 1; y < roiHeight; y++)
			{
				image_index = (roiTop + y) * bytes_per_line + (roiLeft + 1)*bytes_per_pixel;
				dst_index = y * roiWidth + 1;
				for (x = 1; x < roiWidth; x++, image_index += bytes_per_pixel, dst_index++)
				{
					// calc intensity
					intensity = 
						(toGrey ? floatGrey(pSrc + image_offset) : pSrc[image_offset]);

					// calc integral
					pDst[dst_index] =
						pDst[dst_index + x_1y] + 
						pDst[dst_index + xy_1] -
						pDst[dst_index + x_1y_1] + 
						(intensity - vNormalize);
					pVar[dst_index] =
						pVar[dst_index + x_1y] + 
						pVar[dst_index + xy_1] -
						pVar[dst_index + x_1y_1] + 
						(intensity * intensity - sqrV);
				}
			}
			break;
		}
	}


public: /*Binarization*/
	static void Isodata(const unsigned char* imagedata, 
		int bytes_per_pixel, int bytes_per_line, 
		int roiLeft, int roiTop, int roiWidth, int roiHeight, 
		int kernelWidth, int kernelHeight,
		uinT32* pixdata, int wpl)
	{
		int roiLength = roiWidth * roiHeight;
		double* integral = new double[roiLength];

		// this value can be achived by estimating background amplitude with sampling data
		double vNormalize = 160; // this value makes sure that integral at each level is converged to 0
		LocalAdaptiveThreshold::CalcIntegral(
			imagedata, bytes_per_pixel, bytes_per_line, 
			roiLeft, roiTop, roiWidth, roiHeight,
			vNormalize, integral);

		double backgroundAmplitude = (integral[roiLength - 1] / roiLength) + vNormalize;

		double vNormalize_BkgAmplitude = vNormalize + backgroundAmplitude;

		int image_width = bytes_per_line / bytes_per_pixel;		

		int kLength = kernelWidth * kernelHeight;
		int offsetT_1L_1 = 0;
		int offsetT_1R = 0;
		int offsetB_L_1 = 0;
		int offsetBR = 0;
		LocalAdaptiveThreshold::CalcOffsets(
			roiWidth, kernelWidth, kernelHeight, 
			offsetT_1L_1, offsetT_1R, offsetB_L_1, offsetBR);

		int roiOffset = (kernelHeight >> 1) * roiWidth + (kernelWidth >> 1);
		int imgOffset = (kernelHeight >> 1) * bytes_per_line + (kernelWidth >> 1) * bytes_per_pixel;
		int yStart = roiTop + 1;
		int xStart = roiLeft + 1;
		int yEnd = (roiTop + roiHeight) - kernelHeight - 1;
		int xEnd = (roiLeft + roiWidth) - kernelWidth - 1;
		int image_index, integral_index;

		bool toGrey = (bytes_per_pixel == 3 || bytes_per_pixel == 4);
		int r = (toGrey ? (bytes_per_pixel - 1) : 0);
		int g = (toGrey ? (bytes_per_pixel - 2) : 0);
		int b = (toGrey ? (bytes_per_pixel - 3) : 0);

		bool white_result = true;
		double v, threshold;

		int dstPixel = 0;

		for (int y = yStart; y <= yEnd; y++)
		{
			uinT32* pixline = pixdata + (y - roiTop + (kernelHeight >> 1)) * wpl;

			image_index = imgOffset + y * bytes_per_line + xStart * bytes_per_pixel;
			integral_index = (y - roiTop) * roiWidth + 1;
			dstPixel = xStart - roiLeft + (kernelWidth >> 1); 
			for (int x = xStart; 
				x <= xEnd; 
				x++, integral_index++, 
				image_index += bytes_per_pixel,
				dstPixel++)
			{			
				v = 
					integral[integral_index + offsetBR] - 
					integral[integral_index + offsetT_1R] - 
					integral[integral_index + offsetB_L_1] + 
					integral[integral_index + offsetT_1L_1];
				
				threshold = ((v / kLength) + vNormalize_BkgAmplitude) * 0.5;

				white_result = 
					((toGrey ? floatGrey(imagedata + image_index) : imagedata[image_index]) >= threshold);

				if (white_result)
					CLEAR_DATA_BIT(pixline, dstPixel);
				else
					SET_DATA_BIT(pixline, dstPixel);
			}
		}

		// remain regions
		threshold = (integral[roiLength-1] / roiLength) + vNormalize;
		ThresholdRemainRegions(
			imagedata, bytes_per_pixel, bytes_per_line,
			roiLeft, roiTop, roiWidth, roiHeight,
			kernelWidth, kernelHeight, threshold,
			pixdata, wpl);

		delete[] integral;
	}

	static void Sauvola(const unsigned char* imagedata, 
		int bytes_per_pixel, int bytes_per_line, 
		int roiLeft, int roiTop, int roiWidth, int roiHeight, 
		int kernelWidth, int kernelHeight,
		uinT32* pixdata, int wpl)
	{
		// Sauvola's algorithm parameters
		double sauvola_k = 0.34;
		double sauvola_r = 128;

		int roiLength = roiWidth * roiHeight;
		double* integral = new double[roiLength];
		double* varIntegral = new double[roiLength];

		// this value can be achived by estimating background amplitude with sampling data
		double vNormalize = 160; // this value makes sure that integral at each level is converged to 0
		double sqrV = vNormalize * vNormalize;
		LocalAdaptiveThreshold::CalcIntegral(
			imagedata, bytes_per_pixel, bytes_per_line, 
			roiLeft, roiTop, roiWidth, roiHeight,
			vNormalize, integral, varIntegral);

		int image_width = bytes_per_line / bytes_per_pixel;		

		int kLength = kernelWidth * kernelHeight;
		int offsetT_1L_1 = 0;
		int offsetT_1R = 0;
		int offsetB_L_1 = 0;
		int offsetBR = 0;
		LocalAdaptiveThreshold::CalcOffsets(
			roiWidth, kernelWidth, kernelHeight, 
			offsetT_1L_1, offsetT_1R, offsetB_L_1, offsetBR);

		int roiOffset = (kernelHeight >> 1) * roiWidth + (kernelWidth >> 1);
		int imgOffset = (kernelHeight >> 1) * bytes_per_line + (kernelWidth >> 1) * bytes_per_pixel;
		int yStart = roiTop + 1;
		int xStart = roiLeft + 1;
		int yEnd = (roiTop + roiHeight) - kernelHeight - 1;
		int xEnd = (roiLeft + roiWidth) - kernelWidth - 1;
		int image_index, integral_index;

		bool toGrey = (bytes_per_pixel == 3 || bytes_per_pixel == 4);
		int r = (toGrey ? (bytes_per_pixel - 1) : 0);
		int g = (toGrey ? (bytes_per_pixel - 2) : 0);
		int b = (toGrey ? (bytes_per_pixel - 3) : 0);

		double kSqrV = kLength * sqrV;
		double kSub1 = kLength - 1.0;

		bool white_result = true;
		double v, threshold, localMean, localVar;

		int dstPixel = 0;

		for (int y = yStart; y <= yEnd; y++)
		{
			uinT32* pixline = pixdata + (y - roiTop + (kernelHeight >> 1)) * wpl;

			image_index = imgOffset + y * bytes_per_line + xStart * bytes_per_pixel;
			integral_index = (y - roiTop) * roiWidth + 1;
			dstPixel = xStart - roiLeft + (kernelWidth >> 1); 
			for (int x = xStart; 
				x <= xEnd; 
				x++, integral_index++, 
				image_index += bytes_per_pixel,
				dstPixel++)
			{				
				localMean = 
					integral[integral_index + offsetBR] - 
					integral[integral_index + offsetT_1R] - 
					integral[integral_index + offsetB_L_1] + 
					integral[integral_index + offsetT_1L_1];
				localMean = (localMean / kLength) + vNormalize;

				localVar = 
					varIntegral[integral_index + offsetBR] - 
					varIntegral[integral_index + offsetT_1R] - 
					varIntegral[integral_index + offsetB_L_1] + 
					varIntegral[integral_index + offsetT_1L_1];
				localVar = (localVar + kSqrV) / kSub1;

				v = sqrt(localVar - localMean * localMean);
				threshold = localMean * ( 1 + sauvola_k * (-1 + v / sauvola_r));
				
				white_result = 
					((toGrey ? floatGrey(imagedata + image_index) : imagedata[image_index]) >= threshold);

				if (white_result)
					CLEAR_DATA_BIT(pixline, dstPixel);
				else
					SET_DATA_BIT(pixline, dstPixel);
			}
		}

		// remain regions
		threshold = (integral[roiLength-1] / roiLength) + vNormalize;
		ThresholdRemainRegions(
			imagedata, bytes_per_pixel, bytes_per_line,
			roiLeft, roiTop, roiWidth, roiHeight,
			kernelWidth, kernelHeight, threshold,
			pixdata, wpl);

		delete[] integral;
		delete[] varIntegral;
	}

private: /*threshold remain regions*/
	static void ThresholdRemainRegions(const unsigned char* imagedata, 
		int bytes_per_pixel, int bytes_per_line, 
		int roiLeft, int roiTop, int roiWidth, int roiHeight, 
		int kernelWidth, int kernelHeight, double threshold,
		uinT32* pixdata, int wpl)
	{
		bool toGrey = (bytes_per_pixel == 3 || bytes_per_pixel == 4);
		int r = (toGrey ? (bytes_per_pixel - 1) : 0);
		int g = (toGrey ? (bytes_per_pixel - 2) : 0);
		int b = (toGrey ? (bytes_per_pixel - 3) : 0);

		int* pY = NULL, *pX0 = NULL, *pX1 = NULL;
		int n = 0;
		GetRemainScanLines(
			roiLeft, roiTop, roiWidth, roiHeight, 
			kernelWidth, kernelHeight, 
			&pY, &pX0, &pX1, n);

		bool white_result = true;
		int imgOffset = roiTop * bytes_per_line + roiLeft * bytes_per_pixel;
		int dstPixel = 0, y, xStart, xEnd, image_index;
		for (int i=0; i<n; i++)
		{
			y = pY[i];
			xStart = pX0[i];
			xEnd = pX1[i];
			uinT32* pixline = pixdata + (y - roiTop) * wpl;
			image_index = y * bytes_per_line + xStart * bytes_per_pixel;
			dstPixel = xStart - roiLeft;
			for (int x = xStart; 
				x <= xEnd; 
				x++,
				image_index += bytes_per_pixel,
				dstPixel++)
			{
				white_result = 
					((toGrey ? floatGrey(imagedata + image_index) : imagedata[image_index]) >= threshold);

				if (white_result)
					CLEAR_DATA_BIT(pixline, dstPixel);
				else
					SET_DATA_BIT(pixline, dstPixel);
			}
		}

		if (n > 0)
		{
			delete[] pY; delete[] pX0, delete[] pX1;
		}
	}
public: /*Helper*/
	static void CalcOffsets(
		int imageStride, int kWidth, int kHeight,
		int &offsetT_1L_1, int &offsetT_1R, int &offsetB_L_1, int &offsetBR)
	{
		offsetT_1L_1    = -imageStride - 1;
		offsetT_1R      = -imageStride + kWidth - 1;
		offsetB_L_1     = (kHeight - 1) * imageStride - 1;
		offsetBR        = (kHeight - 1) * imageStride + kWidth - 1;
	}

	static void GetRemainScanLines(
		int roiLeft, int roiTop, int roiWidth, int roiHeight,
		int kernelWidth, int kernelHeight,
		int** scy, int** scx0, int** scx1, int &n)
	{
		int yStart = roiTop + (kernelHeight >> 1) + 1;
		int xStart = roiLeft + (kernelWidth >> 1) + 1;
		int yEnd = (roiTop + roiHeight) - (kernelHeight >> 1) - 1;
		int xEnd = (roiLeft + roiWidth) - (kernelWidth >> 1) - 1;

		n = 0;		
		n += yStart - roiTop; // top-region
		n += (roiTop + roiHeight) - (yEnd + 1); // bottom-region
		n += 2 * (yEnd - yStart + 1); //left/right region

		if (n <= 0 || roiWidth < kernelWidth || roiHeight < kernelHeight)
		{
			n = roiHeight;
			yStart = roiTop + roiHeight;
			yEnd = yStart - 1;
		}

		*scy = new int[n]; int* pY = *scy;
		*scx0 = new int[n]; int* pX0 = *scx0;
		*scx1 = new int[n]; int* pX1 = *scx1;
		int index = 0;
		for (int y=roiTop; y<yStart; y++, index++)
		{
			pY[index] = y;
			pX0[index] = roiLeft;
			pX1[index] = roiLeft + roiWidth - 1;
		}
		for (int y=yStart; y<=yEnd; y++)
		{
			pY[index] = y;
			pX0[index] = roiLeft;
			pX1[index] = xStart-1;
			index++;

			pY[index] = y;
			pX0[index ] = xEnd + 1;
			pX1[index] = roiLeft + roiWidth - 1;
			index++;
		}
		for (int y=yEnd+1; y<roiTop+roiHeight; y++, index++)
		{
			pY[index] = y;
			pX0[index] = roiLeft;
			pX1[index] = roiLeft + roiWidth - 1;
		}

		pY = NULL; pX0 = NULL; pX1 = NULL;
	}
};

}  // namespace tesseract.

#endif  // TESSERACT_CCMAIN_LATHR_H__