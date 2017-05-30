.assembly extern mscorlib {}
.assembly Hello.txt {}
.method public void main() {
.maxstack 4
.entrypoint
call void[mscorlib] System.Console::WriteLine()
ldstr "Hello, World"
call void[mscorlib] System.Console::WriteLine(string)
ret
}
