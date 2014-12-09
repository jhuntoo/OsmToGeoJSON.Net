using System.Collections.Generic;
using NUnit.Framework;

namespace OsmToGeoJSON.Tests
{
    [TestFixture]
   public class TagClassifierTests
    {
        [Test]
        public void uninteresting_tags_return_false()
        {
            var tagClassifier = new TagClassifier();
            var uninterestingTags = new Dictionary<string, object>
            {
                {"source", ""},
                {"source_ref", ""},
                {"source:ref", ""},
                {"history", ""},
                {"attribution", ""},
                {"created_by", ""},
                {"tiger:county", ""},
                {"tiger:tlid", ""},
                {"tiger:upload_uuid", ""}
            };
            Assert.IsFalse(tagClassifier.AreInteresting(uninterestingTags));
        }

        [Test]
        public void uninteresting_tags_plus_a_single_interesting_tag_returns_true()
        {
            var tagClassifier = new TagClassifier();
            var uninterestingTags = new Dictionary<string, object>
            {
                {"source", ""},
                {"source_ref", ""},
                {"source:ref", ""},
                {"history", ""},
                {"attribution", ""},
                {"created_by", ""},
                {"tiger:county", ""},
                {"tiger:tlid", ""},
                {"tiger:upload_uuid", ""},
                {"somethinginteresting", "foo"}
            };
            Assert.IsTrue(tagClassifier.AreInteresting(uninterestingTags));
        }

        [Test]
        public void two_interesting_tags_return_true()
        {
            var tagClassifier = new TagClassifier();
            var uninterestingTags = new Dictionary<string, object>
            {
                {"interesting1", "1"},
                {"interesting2", "2"},
            };
            Assert.IsTrue(tagClassifier.AreInteresting(uninterestingTags));
        }

        [Test]
        public void two_interesting_tags_and_one_ignored_returns_true()
        {
            var tagClassifier = new TagClassifier();
            var uninterestingTags = new Dictionary<string, object>
            {
                {"interesting1", "1"},
                {"interesting2", "2"},
            };

            var tagsToIgnore = new Dictionary<string, object>
            {
                {"interesting1", "1"}
            };
            Assert.IsTrue(tagClassifier.AreInteresting(uninterestingTags, tagsToIgnore));
        }

        public void two_interesting_tags_and_both_ignored_returns_false()
        {
            var tagClassifier = new TagClassifier();
            var uninterestingTags = new Dictionary<string, object>
            {
                {"interesting1", "1"},
                {"interesting2", "2"},
            };

            var tagsToIgnore = new Dictionary<string, object>
            {
                {"interesting1", "1"},
                {"interesting2", "2"},
            };
            Assert.IsFalse(tagClassifier.AreInteresting(uninterestingTags, tagsToIgnore));
        }

        [Test]
        public void two_interesting_tags_and_both_ignored_but_match_value_set_to_fales_returns_false()
        {
            var tagClassifier = new TagClassifier();
            var uninterestingTags = new Dictionary<string, object>
            {
                {"interesting1", "1"},
                {"interesting2", "2"},
            };

            var tagsToIgnore = new Dictionary<string, object>
            {
                {"interesting1", "sdf"},
                {"interesting2", "fds"},
            };
            Assert.IsFalse(tagClassifier.AreInteresting(uninterestingTags, tagsToIgnore, false));
        }

        [Test]
        public void two_interesting_tags_and_both_ignored_but_with_different_values_returns_true()
        {
            var tagClassifier = new TagClassifier();
            var uninterestingTags = new Dictionary<string, object>
            {
                {"interesting1", "1"},
                {"interesting2", "2"},
            };

            var tagsToIgnore = new Dictionary<string, object>
            {
                {"interesting1", "one"},
                {"interesting2", "two"},
            };
            Assert.IsTrue(tagClassifier.AreInteresting(uninterestingTags, tagsToIgnore));
        }






    }
}