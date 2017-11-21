
#ifdef _MSC_VER
#define naked __declspec(naked) 
#else
#define naked
#endif

void Run();

int TestArray[8000];
int test = 90;

extern "C"{



	void naked __cdecl kernel_entry() {
		for (size_t i = 0; i < 1024; i++)
		{
			TestArray[i] = 0;
		}
		_asm CLI
		while (true) _asm HLT;
	}

}

void Run() {


}