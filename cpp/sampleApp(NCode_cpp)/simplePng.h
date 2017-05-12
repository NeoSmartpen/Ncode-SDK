#include <string>
#include "pngpp\png.hpp"

using namespace std;
using namespace png;

void MakeSimplePng(string filename, int width, int height, unsigned char *data)
{
	image<gray_pixel_1> image(width, height);

	for (size_t y = 0; y < image.get_height(); ++y)
	{
		for (size_t x = 0; x < image.get_width(); ++x)
		{
			gray_pixel_1 pix;

			size_t index = (width * y + x) / 8;
			size_t index2 = 7 - ((width * y + x) % 8);
			byte v = (0x01 << index2);
			if (data[index] & v)
				pix = 0;
			else
				pix = 1;

			image[y][x] = pix;
		}
	}

	image.write(filename);
}

