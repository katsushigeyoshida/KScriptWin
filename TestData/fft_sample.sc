//	FFT
n = 5000;						//	サンプリング数
dt = 0.0001;					//	サンプリング間隔(s)
fs = 1 / dt;					//	サンプリング周波数(Hz)
t[] = array.arrange(0,n*dt,dt);
println("サンプリング数    : ",n," \nサンプリング間隔  : ",dt,"s \nサンプリング周波数: ",fs," Hz");
f1 = 50;
f2 = 220;
signal[] = t[];
express = "sin(2*PI*f1*[x])+0.5*sin(2*PI*f2*[x])";
array.calc(signal[],express);

//	波形グラフ表示
graph.SetSplitArea(1,2);
graph.SetUseArea(0);
graph.GraphType("line");
//	データ抽出サイズ
size = 500;
t2[] = array.copy(t[],0,size);
signal2[] = array.copy(signal[],0,size);
//	データグラフ
graph.SetData(t2[],signal2[]);

//	FFT Magnitude
mag[] = math.fourier(signal[]);
mag2[] = array.copy(mag[],0,size);
t2[] = array.arrange(0,size,2);
graph.SetUseArea(1);
graph.SetData(t2[],mag2[]);

//for(i=0; i<n; i+= 10) {
//	y = sin(2*PI*f1*t[i])+0.5*sin(2*PI*f2*t[i]);
//	println(t[i]," ",signal[i]);
//}
