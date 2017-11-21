

#include "stdafx.h"

#ifdef _UNIT
#include <iostream>
#include <string>
#include "stdafx.h"
#include "Compiler.h"
#include "ByteCode.h"
#include  <iomanip>
#include <malloc.h>
#include <assert.h>
using namespace std;
#define Section    



int main()
{
	cout << "UNIT tests\n";


	cout << "\nPress Any key To exit";
	cin.get();
}

unsigned int Factorial(unsigned int number) {
	return number <= 1 ? number : Factorial(number - 1)*number;
}
#endif







