#include "cbasetypes.h"

#define MAXKEYBYTES 56          /* 448 bits */
#define little_endian 1              /* Eg: Intel */
//#define big_endian 1            /* Eg: Motorola */

#ifdef __cplusplus
extern "C"
{
#endif
	int16 opensubkeyfile(void);
	uint32 F(uint32 x);
	__declspec(dllexport) void blowfish_encipher(uint32 *xl, uint32 *xr, uint32 * P);
	__declspec(dllexport) void blowfish_decipher(uint32 *xl, uint32 *xr, uint32 * P);
	__declspec(dllexport) short initializeBlowfish(char key[], int16 keybytes, uint32 * P);
#ifdef __cplusplus
}
#endif
