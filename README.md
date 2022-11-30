# Socket Demo
Shows how using close connection header prevents sockets from being stuck in the TIME_WAIT on the client
![image](https://user-images.githubusercontent.com/117949398/204637418-7a212d30-6dd2-44bc-8270-49eee58e062f.png)


# Connection Timer

Compare local vs external http and https with force close connection and keep-alive header flags 

![image](https://user-images.githubusercontent.com/117949398/204843640-f7c0be38-05b3-4263-be5d-0f4294b94dec.png)

These results indicate almost all the aditional overhead is related to network latency. There is virtually no difference when running this test on localhost, using http or https. In [this](http://netsekure.org/2010/03/tls-overhead/ ) article we can see that the average TLS connection uses around 6.5k bytes of data where as resuming a connection uses only 330 bytes. Most this difference is related to the exchange of the certificate and this additional exchange performed on the initial handshake. 

### TLS standard handshake flow

![image](https://user-images.githubusercontent.com/117949398/204861882-d9cc4f1c-89dc-4565-b2c1-4f064c1d9f66.png)

### TLS resume existing connection flow

![image](https://user-images.githubusercontent.com/117949398/204861973-84ab01a6-413e-48b8-b724-d45e27da983b.png)
