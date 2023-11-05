# Memory.Extensions [![NuGet](https://img.shields.io/nuget/v/Yotic.Memory.Extensions.svg)](https://www.nuget.org/packages/Yotic.Memory.Extensions)

Memory utils for convenient work with memory, arrays and pointers.\
The library is mainly intended for use with native code, such as C++ or in conjunction with Native AOT. All actions are carried out with the heap, all allocated pointers do not change the address, so they can be safely used from any part of the code

Allocating memory
------------------------------
```C#
void* ptr = MemEx.Alloc(6); // Allocates 6 bytes in memory
// ...
MemEx.Free(ptr); // Free pointer
```
You can use other overloads of Alloc method for convenient work
```C#
int* ptr = MemEx.Alloc<int>(6); // Allocates sizeof(int) * 6 bytes in memory
```
```C#
int value = 7;
int* ptr = MemEx.NewAlloc(value); // Allocates 4 bytes and write value to pointer
// It should be noted that the allocation goes to the heap, not the stack, so this is not the same as &value
```
```C#
int[] nums = { 1, 2, 3 };
int* ptr = MemEx.AllocFrom(nums); // Allocates sizeof(int) * nums.Length bytes in memory and write array to pointer
```

Allocating strings
------------------------------
```C#
string managedString = "Some unicode string";
using CoMem strCo = new CoMem(managedString, CoStrType.Uni);

ushort* unmanagedStringPtr = (ushort*)strCo;
//...
```
