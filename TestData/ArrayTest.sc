#include "scriptLib.sc";

a["apple"] = 50;
println(a["apple"]);


address[“yamada”,] = { “tarou”, 60, “men”};
address[“satou”,] = { “hanako”, 30, “women”};

name = "yamada";
println(address[yamada, 0]);

a[] = { “name”, 18, “men”};
println(a[1]);

exit;

a[] = { 1, 2, 3, 4, 5 };
b[] = { 2, -2, 13, -4, 5 };
print("配列 a[] = "); printArray(a[]);
print("配列 b[] = "); printArray(b[]);
println("配列 a[] の数と合計 = ", count(a[]), ", ", sum(a[]));
println("配列 b[] の数と合計 = ", count(b[]), ", ", sum(b[]));
println("配列 a[] の最小最大 = ", min(a[]), ", ", max(a[]));
println("配列 b[] の最小最大 = ", min(b[]), ", ", max(b[]));
println("配列 a[] の平均 = ", average(a[]));
println("配列 b[] の平均 = ", average(b[]));
println("配列 a[] の分散 = ", variance(a[]));
println("配列 b[] の分散 = ", variance(b[]), " ", round(variance(b[])));
println("配列 a[] の標準偏差 = ", stdDeviation(a[]));
println("配列 b[] の標準偏差 = ", stdDeviation(b[]));
b[]= sort(b[]);
print("配列b[]をソート = "); printArray(b[]);
b[] = reverse(b[]);
print("配列b[]を逆順 = "); printArray(b[]);
