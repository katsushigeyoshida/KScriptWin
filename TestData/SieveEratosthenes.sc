//	SieveEratosthenes
print("エラトステネスの篩\n");
max = inputBox("素数の最大値");
maxprime = floor(sqrt(max));
print("素数の最大値 = ", max, "  最大篩値 = ", maxprime, "\n");
print("素数テーブル", "\n");
for (n = 2; n < max; n = n + 1)
    a[n] = 1;

m = 2;
while (m < maxprime) {
    if (a[m] == 1) {
        print(m," : ");
        for (n = m + m; n < max; n += m)
            a[n] = 0;
        for (n = 2; n < max; n++) {
            if (a[n] == 1)
                print(n, " ");
        }
        print();
    }
    m = m + 1;
}