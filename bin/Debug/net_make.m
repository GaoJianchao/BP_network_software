function [XDE3,inputWeights,inb,outw,outbias]= net_make(m_data,yn,t_data,s,dl,dc,wc)
%UNTITLED 此处显示有关此函数的摘要
%
%m_data=xlsread(filename);
p1=m_data(:,1:(size(m_data,2)-1));
t1=m_data(:,size(m_data,2));
p=p1';
t=t1';
[pn,minp,maxp,tn,mint,maxt]=premnmx(p,t);
n=yn;
net=newelm(minmax(pn),[n,1],{'tansig','purelin'});

inputWeights=net.IW{1,1};
inputbias=net.b{1};
inb=inputbias';
outWeights=net.LW(2,1);
outw=outWeights{1};
outbias=net.b{2};

net.trainParam.show=20;
net.trainParam.lr=s;
net.trainParam.mc=dl;
net.trainParam.epochs=dc;
net.trainParam.goal=wc;
net=init(net);
net=train(net,pn,tn);

%A=sim(net,pn);
%result=postmnmx(A,mint,maxt);
%JDN=result-t;
%SSE=sse(JDN);
%MSE=mse(JDN);
%XDE=JDN./t;
%fprintf('%f  %f',SSE,MSE);
%disp(t)
%disp(result)
%disp(XDE)

%testname
%t_data=xlsread(testname);
p2=t_data(:,1:(size(t_data,2)-1));
t2=t_data(:,size(t_data,2));
tp=p2';
tt=t2';
p2n=tramnmx(tp,minp,maxp);
a2n=sim(net,p2n);
result2=postmnmx(a2n,mint,maxt);
new_JDN=result2-tt;
XDE2=new_JDN./tt;
SSE2=sse(new_JDN);
MSE2=mse(new_JDN);
result=[p2 t2 result2' XDE2'];
XDE3=result;
%disp(tt);
%disp(result2);
%disp(XDE2)
%fprintf('%f  %f\n',SSE2,MSE2);
%figure(1);
%plot(1:1:3,result2,'-ro','linewidth',2);
%hold on;
%plot(1:1:3,tt,'k-.s','linewidth',2);
%hold on;
%title('o为真实值，s为预测值');

end

