namespace EasyDockerFile.Core.Types;

public enum PackageType 
{
    Admin,              // Package enabling administrative functionality. 
    Boot,               // Package responsible for boot functionality.
    Common,             // A common file like a log or temporary file associated with a package.
    Database,           // Package related to the installation of DB engines and related applications.
    Debug,              // Package containing debug symbols
    Documentation,      // Package containing Linux command or kernel documentation.
    Editor,             // Package related to the installation of a code editor.
    Education,          // Package related to educational applications.
    Electronics,        // Package related to the operation of other electronics through the onboard bus.
    Embedded,           // Package related to development on embedded systems.
    Fonts,              // Package related to the installation of Fonts.
    Games,              // Package related to the installation of a game.
    GeneralDevelopment, // Package for developers, containing specific headers.
    Golang,             // Package related to the installation of, and development with, golang.
    Gnome,              // Package related to the Gnome Desktop Environment.
    GnuR,               //
    GnuStep,            // 
    Graphics,           // Package related to general graphical functionality.
    Haskell,            // Package related to haskell development.
    HamRadio,           // Package related to the operation of HamRadio.
    HttpD,              // Package related to the Linux HTTP Daemon.
    Interpreters,       // Package related to compilation/intepreting of code.
    Introspection,      // Package
    Java,               // Package related to the installation of Java or Java development.
    JavaScript,         // Package related to the installation of JavaScript or JavaScript development.
    KDE,                // Package related to the KDE Desktop Environment.
    Kernel,             // Package responsible for kernel functionality.
    LegacyLibrary,      // Package that was categorized as a legacy library; it remains due for legacy compatibility.
    Library,            // A standard library/utility package.
    LibraryDevelopment, // Package specific to the development of common libraries.
    Lisp,               // Package related to the installation of Lisp and Lisp development.
    Localization,       // Package related to general locale and region data.
    Mail,               // Package related to the installation of a mail client or other associated functionality.
    Math,               // Package related to computational operations or more specialized calculation tasks.
    MetaPackages,       // Package containing no binary data, only references to other packages; these are used for simplifying installations.
    Misc,               // Package that can not be categorized within the other types.
    Mono,               // Package related to the installation of the Mono Runtime.
    Networking,         // Package responsible for handling networking functionality.
    News,               // Package for Desktop News Applications.
    Ocaml,              // Package on the Ocaml.org Package Manager.
    OtherOSFS,          // Package for cross-platform filesystem operations.
    Perl,               // Package related to the installation of Perl or Perl development.
    PHP,                // Package related to the installation of PHP, or PHP development.
    Python,             // Package related to Python installation and development.
    Ruby,               // Package related to Ruby installation, and development.
    Rust,               // Package related to Rust installation, and development.
    Science,            // Package related to scientific research.
    Shells,             // Package related to shell execution or alternative shells such as zsh or fish.
    Sound,              // Package related to basic sound functionality and more specialized sound engineering tasks.
    Tex,                // Package for the installation, operation, and development with LaTeX.
    Text,               // Package for the installation of a text editor.
    Utility,            // Package that is classified as a general system utility.
    VCS,                // Package for a Version Control Softwares (like git, fossil, etc).
    Video,              // Package for the installation and specialized functionality of video software
    Web,                // Package for general web operations on Linux
    X11,                // Package related to the functionality and installation of the X11 server and general functionality.
    XFCE,               // Package related to the XFCE Desktop Environment.
    Zope,               // Package related to Zope Proxy.
    Unknown             // Unidentified Package

}