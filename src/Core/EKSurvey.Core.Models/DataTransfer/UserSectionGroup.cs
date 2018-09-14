using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using EKSurvey.Core.Extensions;
using EKSurvey.Core.Models.Enums;
using EKSurvey.Core.Models.Exceptions;

namespace EKSurvey.Core.Models.DataTransfer
{
    public class UserSectionGroup : IList<UserSection>, IUserSection
    {
        private readonly List<UserSection> _collection = new List<UserSection>();

        // ReSharper disable once UnusedMember.Global -- needed for mapping
        public UserSectionGroup() { }

        public UserSectionGroup(IEnumerable<UserSection> collection)
        {
            _collection = (collection ?? throw new ArgumentNullException(nameof(collection))).ToList();
            ThrowIfInvalid();
        }
        private void ThrowIfInvalid()
        {
            if (!_collection.Select(i => i.UserId).IsUnanimous())
                throw new InvalidSectionGroupConfigurationException("Sections are not owned by the same user id.");
            if (!_collection.Select(i => i.SurveyId).IsUnanimous())
                throw new InvalidSectionGroupConfigurationException("Sections are not associated to a single survey id.");
            if (!_collection.Select(i => i.TestSectionMarkerId).IsUnanimous())
                throw new InvalidSectionGroupConfigurationException("Sections are not associated to a single test id.");
            if (!_collection.Select(i => i.Order).IsUnanimous())
                throw new InvalidSectionGroupConfigurationException("Sections must all belong to the same order.");
        }

        public string UserId => _collection.FirstOrDefault()?.UserId;
        public int SurveyId => (_collection.FirstOrDefault()?.SurveyId).GetValueOrDefault();
        public Guid TestId => (_collection.FirstOrDefault()?.TestId).GetValueOrDefault();
        public Guid? TestSectionMarkerId => SelectedSection?.TestSectionMarkerId;
        public int Order => (_collection.FirstOrDefault()?.Order).GetValueOrDefault();

        public int? Id => SelectedSection?.Id;

        public DateTime? Started => _collection.FirstOrDefault(i => i.Started.HasValue)?.Started;
        public DateTime? Modified => _collection.FirstOrDefault(i => i.Modified.HasValue)?.Modified;
        public DateTime? Completed => _collection.FirstOrDefault(i => i.Completed.HasValue)?.Completed;
        public UserSection this[int index]
        {
            get => _collection[index];
            set => _collection[index] = value;
        }
        public int Count => _collection.Count;
        public bool IsReadOnly => false;

        public UserSection SelectedSection => _collection.SingleOrDefault(us => us.Started.HasValue && !us.Completed.HasValue);

        public SelectorType? SelectorType { get; set; }

        public IEnumerator<UserSection> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(UserSection item)
        {
            if (_collection.Contains(item, UserSectionComparer.Default))
            {
                var existing = _collection.Single(i => UserSectionComparer.Default.Equals(i, item));
                _collection.Remove(existing);
            }

            _collection.Add(item);

            ThrowIfInvalid();
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public bool Contains(UserSection item)
        {
            return _collection.Contains(item, UserSectionComparer.Default);
        }

        public void CopyTo(UserSection[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(UserSection item)
        {
            return _collection.Remove(item);
        }

        public int IndexOf(UserSection item)
        {
            return _collection.IndexOf(item);
        }

        public void Insert(int index, UserSection item)
        {
            _collection.Insert(index, item);
            ThrowIfInvalid();
        }

        public void RemoveAt(int index)
        {
            _collection.RemoveAt(index);
        }

        public void AddRange(IEnumerable<UserSection> collection)
        {
            foreach (var item in collection)
            {
                _collection.Add(item);
            }

            ThrowIfInvalid();
        }

        internal class UserSectionComparer : IEqualityComparer<UserSection>
        {
            public static UserSectionComparer Default => new UserSectionComparer();

            public bool Equals(UserSection x, UserSection y)
            {
                if (x == null && y == null)
                    return true;

                if (x == null || y == null)
                    return false;

                return x.Id == y.Id;
            }

            public int GetHashCode(UserSection obj)
            {
                return obj.Id.GetHashCode();
            }
        }

    }
}