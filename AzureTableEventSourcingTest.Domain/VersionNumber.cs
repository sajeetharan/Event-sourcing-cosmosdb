using System;

namespace AzureTableEventSourcingTest.Domain
{
	public class VersionNumber: IEquatable<VersionNumber>
	{
        public static VersionNumber None { get; } = new VersionNumber(0);

		public VersionNumber(int value)
		{
			Value = value;
		}

		public int Value { get; }

        public VersionNumber Next => new VersionNumber(Value + 1);

		public override int GetHashCode() => Value.GetHashCode();

		public override bool Equals(object obj) => Equals(obj as VersionNumber);

		public bool Equals(VersionNumber other) => Value == other?.Value;
	}
}
