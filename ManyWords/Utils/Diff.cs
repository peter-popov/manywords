using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace ManyWords.Utils
{
    /// <summary>
    /// Diff algorithm.
    /// Adopt algorithm from here: http://devdirective.com/post/91/creating-a-reusable-though-simple-diff-implementation-in-csharp-part-1
    /// No lince mentioned there.
    /// </summary>
    public class DiffAlg
    {
        /// <summary>
        /// Diff sectin type
        /// </summary>
        public enum DiffSectionType
        {
            Copy,
            Insert,
            Delete
        }


        /// <summary>
        /// Diff section
        /// </summary>
        public struct DiffSection
        {
            public readonly DiffSectionType Type;
            public readonly int Length;
         
            public DiffSection(DiffSectionType type, int length)
            {
                this.Type = type;
                this.Length = length;
            }

            public override string ToString()
            {
                return string.Format("{0} {1}", Type, Length);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstCollection"></param>
        /// <param name="firstStart"></param>
        /// <param name="firstEnd"></param>
        /// <param name="secondCollection"></param>
        /// <param name="secondStart"></param>
        /// <param name="secondEnd"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public static IEnumerable<DiffSection> Diff<T>(
            IList<T> firstCollection, int firstStart, int firstEnd,
            IList<T> secondCollection, int secondStart, int secondEnd,
            IEqualityComparer<T> equalityComparer)
        {
            var lcs = FindLongestCommonSubstring(
                firstCollection, firstStart, firstEnd,
                secondCollection, secondStart, secondEnd,
                equalityComparer);

            if (lcs.Success)
            {
                // deal with the section before
                var sectionsBefore = Diff(
                    firstCollection, firstStart, lcs.PositionInFirstCollection,
                    secondCollection, secondStart, lcs.PositionInSecondCollection,
                    equalityComparer);
                foreach (var section in sectionsBefore)
                    yield return section;

                // output the copy operation
                yield return new DiffSection(
                    DiffSectionType.Copy,
                    lcs.Length);

                // deal with the section after
                var sectionsAfter = Diff(
                    firstCollection, lcs.PositionInFirstCollection + lcs.Length, firstEnd,
                    secondCollection, lcs.PositionInSecondCollection + lcs.Length, secondEnd,
                    equalityComparer);
                foreach (var section in sectionsAfter)
                    yield return section;

                yield break;
            }

            // if we get here, no LCS

            if (firstStart < firstEnd)
            {
                // we got content from first collection --> deleted
                yield return new DiffSection(
                    DiffSectionType.Delete,
                    firstEnd - firstStart);
            }
            if (secondStart < secondEnd)
            {
                // we got content from second collection --> inserted
                yield return new DiffSection(
                    DiffSectionType.Insert,
                    secondEnd - secondStart);
            }
        }

        #region LCS
        /// <summary>
        /// LCS algorithms
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstCollection"></param>
        /// <param name="firstStart"></param>
        /// <param name="firstEnd"></param>
        /// <param name="secondCollection"></param>
        /// <param name="secondStart"></param>
        /// <param name="secondEnd"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        private static LongestCommonSubstringResult FindLongestCommonSubstring<T>(
            IList<T> firstCollection, int firstStart, int firstEnd,
            IList<T> secondCollection, int secondStart, int secondEnd,
            IEqualityComparer<T> equalityComparer)
        {
            // default result, if we can't find anything
            var result = new LongestCommonSubstringResult();

            for (int index1 = firstStart; index1 < firstEnd; index1++)
            {
                for (int index2 = secondStart; index2 < secondEnd; index2++)
                {
                    if (equalityComparer.Equals(
                        firstCollection[index1],
                        secondCollection[index2]))
                    {
                        int length = CountEqual(
                            firstCollection, index1, firstEnd,
                            secondCollection, index2, secondEnd,
                            equalityComparer);

                        // Is longer than what we already have --> record new LCS
                        if (length > result.Length)
                        {
                            result = new LongestCommonSubstringResult(
                                index1,
                                index2,
                                length);
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Check anout of equal elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstCollection"></param>
        /// <param name="firstPosition"></param>
        /// <param name="firstEnd"></param>
        /// <param name="secondCollection"></param>
        /// <param name="secondPosition"></param>
        /// <param name="secondEnd"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        private static int CountEqual<T>(
            IList<T> firstCollection, int firstPosition, int firstEnd,
            IList<T> secondCollection, int secondPosition, int secondEnd,
            IEqualityComparer<T> equalityComparer)
        {
            int length = 0;
            while (firstPosition < firstEnd
                && secondPosition < secondEnd)
            {
                if (!equalityComparer.Equals(
                    firstCollection[firstPosition],
                    secondCollection[secondPosition]))
                {
                    break;
                }

                firstPosition++;
                secondPosition++;
                length++;
            }
            return length;
        }


        /// <summary>
        /// Result for the LCS algorithm.
        /// </summary>
        private struct LongestCommonSubstringResult
        {
            public readonly bool Success;
            public readonly int PositionInFirstCollection;
            public readonly int PositionInSecondCollection;
            public readonly int Length;

            public LongestCommonSubstringResult(
                int positionInFirstCollection,
                int positionInSecondCollection,
                int length)
            {
                Success = true;
                PositionInFirstCollection = positionInFirstCollection;
                PositionInSecondCollection = positionInSecondCollection;
                Length = length;
            }

            public override string ToString()
            {
                if (Success)
                    return string.Format(
                        "LCS ({0}, {1} x{2})",
                        PositionInFirstCollection,
                        PositionInSecondCollection,
                        Length);
                else
                    return "LCS (-)";
            }
        }
        #endregion
    }
}
