using System;

namespace CSharpIdentifiers.Access.Parameters
{
    internal class Lambda
    {
        public void Create()
        {
            Func<int, bool> isNegative = (int arg) => arg < 0;
            // Check argument usage in a ParenthesizedLambdaExpression
            Func<int, bool> isPositive = (int arg) => { return arg < 0; };

            Console.WriteLine(isNegative(25));
            Console.WriteLine(isPositive(25));
        }
    }
}