# PR LAB 2 

**Task:**  
### Implement a protocol atop UDP
1. With a method to make it more reliable, using either (1) error checking + retransmission (this is simple) or (2) error correction (a bit harder but cooler)
2. Make the connection secure, using either symmetric streaming or asymmetric encryption
3. Then, once your protocol is more reliable and secure, make an application-level protocol on top of it, like FTP, or HTTP:
4. A set of methods/verbs and rules on how to interact with it
5. Model the protocol as a state machine, for documentation
6. To prove that everything is working as intended, make a server and a client using this nice protocol of yours.

### My package structure
| message length | message id     | encrypted message  |
|----------------|----------------|--------------------|
| first 2 bytes  | second 2 bytes | rest of the buffer |

### Explanation is in the code comments

![](./work.jpg)