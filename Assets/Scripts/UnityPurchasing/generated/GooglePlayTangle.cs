// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("/syo3Aop22PZNmukIVHgfrPGB2Vpoj6Zw2QV77yCs1INMN4GbdIo4adYndVmd9En1Kq482hx8sAdZ6FpJTtL9Ou0v8+9/U49Qjy+5RcD9fTAnum4aBbiiGRs22iTMFuiUgNKjM17FYu+oLS8JtvqoEGdLkx0StUtH4m6Q9wAoDAJd1Wm6tfCPKGox8S3HNNEuBmNQgdjhPDvX56Da9N/Abmu7qcWvBdvPfYLPmtXA2dzQg4C/NO3Y44Y8cSmNq4cwWh0BMYF2g6yQWDqSPlFTYW3QPmk03w/FskK8zqICyg6BwwDIIxCjP0HCwsLDwoJQbYrtfhbyPwLq8Iu3ulkSYa20bqICwUKOogLAAiICwsKn1kK5f8JacXZIN9YbfJbOwgJCwoL");
        private static int[] order = new int[] { 9,13,4,12,12,7,11,12,10,10,12,12,12,13,14 };
        private static int key = 10;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
