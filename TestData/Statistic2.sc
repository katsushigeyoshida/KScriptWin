a[] =  { 30, 50, 70, 40, 80 };
println("データ : ",a[]);
println("合計 : ",math.sum(a[]));
println("平均 : ",math.average(a[]));
std = math.stdDeviation(a[]);
println("標準偏差 : ",std);

x[] = {  2,  4,  7,  8,  9 };
y[] = { 30, 50, 70, 40, 80 };
cov = math.covariance(x[],y[]);
xstd = math.stdDeviation(x[]);
ystd = math.stdDeviation(y[]);
println("共分散 : ",cov," xの標準偏差 : ",xstd," yの標準偏差 : ",ystd);
cor = math.corrCoeff(x[],y[]);
println("相関係数 : ",cor);
