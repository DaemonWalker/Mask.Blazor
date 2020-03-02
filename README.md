# Mask.Blazor
一个比较基础的Blazor+Redis+Hangfire的例子

## Blazor
一个基于.Net Core的WebAssembly框架，可以用C#写Html网页了。我是觉得挺好用的，虽然现在WebAssembly都用来挖矿了-_-||

目前有登录，身份验证，数据绑定，组件，POST这些常用的东西，怎么调JS没用，突然间不是很想写JS也不想去调。

## Redis
本来想用MySQL来着，但是突然想学点新东西，就用的Redis了。但是.Net Core下的StackExchange.Redis实在是。。。
第一次调用就超时所以只能换成了CSRedis，不过现在看或许换对了，CSRedis还是挺好用的，就是封装有点少，不过这都不是事~

## Hangfire
Qutarz.Net二选一，反正都是定时任务，然后对于时间精度又不是那么高，所以抓阄抓出来的。Hangfire的Dashboard很好用，就是当初怎么用内存存储费了半天劲。。。

有一说一，Hangfire的时区真是有点坑。。。不知道是什么的原因，本地没问题，但是在Docker容器里面全是UTC时间，必须使用
`TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");`才能获取北京时间就很头疼。。。

## 胡言乱语时间
要运行的话，给CommonValue.ConnectionString和CommonValue.HangfireDashboard赋下值就行~
