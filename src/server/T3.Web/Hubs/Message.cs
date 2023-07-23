namespace T3.Web.Hubs;

public record Message<T>(T Content, MessageSignature Signature);
public record MessageSignature(MessageSignatureVersion Version, string Payload, string Signature, string PublicKey);

public enum MessageSignatureVersion
{
    V1 = 1
}