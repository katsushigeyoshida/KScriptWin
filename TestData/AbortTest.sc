println("inKey start");
print("Key In ? =");
a = inKey();
println("Key Code: ",a);

s = 20;
println("スリープ ", s, " m sec");
startTime();
sleep(s);
println(lapTime());

print("Abort Test Start\n");
a = 1;
while (a > 0) {
    print("a = ", a, "\n");
    a = a + 1;
}
print("Test End\n");