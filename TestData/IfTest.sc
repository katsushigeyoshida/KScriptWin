a = 2;
b = 3;
while (a <= b + 1) {
    print(a, " ", b, " → ");
    if (a > b) { println(a); }
    else if ( a == b) { println(a, " ", b); }
    else { println(b); }
    a++;
}

a = 2;
b = 3;
while (a <= b) {
    print(a, " ", b, " → ");
    if (a > b) println(a);
    else println(b);
    a++;
}

print(a, " ", b, " → ");
if (a > b) println(a);

