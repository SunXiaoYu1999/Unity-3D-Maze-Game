//-------------------------------------------------
//                    TNet 3
// Copyright © 2012-2020 Tasharen Entertainment Inc
//-------------------------------------------------

namespace TNet
{
    /// <summary>
    /// Helper enum -- the entries should be in the same order as in the Packet enum.
    /// </summary>

    [DoNotObfuscate]
    public enum Target
    {
        /// <summary>
        /// Echo the packet to everyone in the room.
        /// </summary>

        All,

        /// <summary>
        /// Echo the packet to everyone in the room and everyone who joins later.
        /// </summary>

        AllSaved,

        /// <summary>
        /// Echo the packet to everyone in the room except the sender.
        /// </summary>

        Others,

        /// <summary>
        /// Echo the packet to everyone in the room (except the sender) and everyone who joins later.
        /// </summary>

        OthersSaved,

        /// <summary>
        /// Echo the packet to the room's host.
        /// </summary>

        Host,

        /// <summary>
        /// Broadcast is the same as "All", but it has a built-in spam checker. Ideal for global chat.
        /// </summary>

        Broadcast,

        /// <summary>
        /// Send this packet to administrators.
        /// </summary>

        Admin,
    }
}
