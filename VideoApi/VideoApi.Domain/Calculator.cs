namespace VideoApi.Domain
{
    public class Calculator
    {
        public int Add(int num1, int num2)
        {
            return num1 + num2;
        }
        
        public int Subtract(int num1, int num2)
        {
            return num1 - num2;
        }
        
        public int Multiply(int num1, int num2)
        {
            return num1 * num2;
        }
        
        public int Divide(int num1, int num2)
        {
            return num1 / num2;
        }
    }
}
