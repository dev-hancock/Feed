namespace Feed.interfaces;

public interface IValidator<in T>
{
    void Validate(T request);
}