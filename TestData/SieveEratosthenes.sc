//	SieveEratosthenes
n = inputBox("素数チェック");

for (i = 2; i <= n; i++) {
	m = 2^i +1;
	if (primeChk(m) ==1)
		println(i, " ", m, " is prime");
}
//exit;

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


primeChk(n) {
	if (2 <= n) {
		limit = floor(sqrt(n));
		for (i = limit; i > 1; i--) {
			if (n % i == 0) break;
		}
		if (i == 1)
			return 1;
		else
			return 0;
	}
}
