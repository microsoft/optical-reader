/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System.Threading.Tasks;

namespace OpticalReaderApp
{
    public class CustomProcessor : OpticalReaderLib.BasicProcessor
    {
        public CustomProcessor() : base(new OpticalReaderLib.ZxingDecoder())
        {
            Normalizer = new OpticalReaderLib.BasicNormalizer();
            Enhancer = new CustomEnhancer();
        }
    }
}
