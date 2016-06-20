﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:hf="urn:hafnium"
    xmlns:xls="urn:hafnium/excel"
    xmlns:eo="urn:eo-util"
    exclude-result-prefixes="msxsl hf xls eo">

    <xsl:param name="FileName" />
    <xsl:param name="Namespace" />
    <xsl:param name="UriDirectory" />

    <xsl:output method="text" indent="yes" />

    <xsl:variable name="fwk-ns" select=" 'http://copper.com/fwk/' " />
    <xsl:variable name="NewLine">
        <xsl:text>
</xsl:text>
    </xsl:variable>


    <!-- ================================================================
    ~
    ~ rule
    ~
    ================================================================= -->
    <xsl:template match=" hf:rule ">
        <xsl:text>// autogenerate: do NOT manually edit
using Hafnium;
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;
</xsl:text>

        <xsl:text>
namespace </xsl:text>
        <xsl:value-of select=" $Namespace " />
        <xsl:text>
{</xsl:text>
        <xsl:value-of select=" $NewLine " />

        <!-- Request -->
        <xsl:text>    /// &lt;summary&gt;</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:text>    /// Request message for flow.</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:text>    /// &lt;/summary&gt;</xsl:text>
        <xsl:value-of select=" $NewLine " />

        <xsl:text>    public class </xsl:text>
        <xsl:value-of select=" $FileName " />
        <xsl:text>Request</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:text>    {</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:apply-templates select=" hf:request/* " mode="prop" />
        <xsl:text>    }</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:value-of select=" $NewLine " />
        <xsl:value-of select=" $NewLine " />


        <!-- Response -->
        <xsl:text>    /// &lt;summary&gt;</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:text>    /// Response message for flow.</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:text>    /// &lt;/summary&gt;</xsl:text>
        <xsl:value-of select=" $NewLine " />

        <xsl:text>    public class </xsl:text>
        <xsl:value-of select=" $FileName " />
        <xsl:text>Response</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:text>    {</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:apply-templates select=" hf:response/* " mode="prop" />
        <xsl:text>    }</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:value-of select=" $NewLine " />
        <xsl:value-of select=" $NewLine " />


        <!-- Rule -->
        <xsl:text>    /// &lt;summary&gt;</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:text>    /// ?</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:text>    /// &lt;/summary&gt;</xsl:text>
        <xsl:value-of select=" $NewLine " />

        <xsl:text>    [RuleEngine( "</xsl:text>
        <xsl:value-of select=" hf:engine " />
        <xsl:text>" )]</xsl:text>
        <xsl:value-of select=" $NewLine " />

        <xsl:text>    public partial class </xsl:text>
        <xsl:value-of select=" $FileName " />
        <xsl:text> : BaseRule, IRule&lt;</xsl:text>
        <xsl:value-of select=" $FileName " />
        <xsl:text>Request, </xsl:text>
        <xsl:value-of select=" $FileName " />
        <xsl:text>Response&gt;</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:text>    {</xsl:text>
        <xsl:value-of select=" $NewLine " />
        <xsl:text>    }</xsl:text>
        <xsl:value-of select=" $NewLine " />

        <xsl:text>
}</xsl:text>
    </xsl:template>



    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ cu:* / mode=prop
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" * " mode="prop">
        <xsl:call-template name="summary">
            <xsl:with-param name="indent" select=" '        ' " />
        </xsl:call-template>
        <xsl:value-of select=" $NewLine " />
        <xsl:apply-templates select=" . " mode="prop-rule" />
        <xsl:apply-templates select=" . " mode="type-attr" />
        <xsl:apply-templates select=" . " mode="rmap-attr" />
        <xsl:text>        public </xsl:text>
        <xsl:apply-templates select=" . " mode="type" />
        <xsl:text> </xsl:text>
        <xsl:value-of select=" @name " />
        <xsl:text> { get; set; }</xsl:text>
        <xsl:value-of select=" $NewLine " />

        <xsl:if test=" position() != last() ">
            <xsl:value-of select=" $NewLine " />
        </xsl:if>
    </xsl:template>

    <xsl:template match=" hf:summary " mode="prop" />


    <!-- ================================================================
    ~
    ~ Library!
    ~
    ================================================================= -->

    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ cu:* / mode=type
    ~ Responsible for emitting the .NET type, be it a native CLR type or
    ~ a custom defined type.
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" * " mode="type">
        <xsl:text>undef /*</xsl:text>
        <xsl:value-of select=" local-name(.) " />
        <xsl:text> */</xsl:text>
    </xsl:template>

    <xsl:template name="type-native">
        <xsl:choose>
            <xsl:when test=" @array = 'true' ">
                <xsl:text>[]</xsl:text>
            </xsl:when>
            <xsl:when test=" @optional = 'true' ">
                <xsl:text>?</xsl:text>
            </xsl:when>
        </xsl:choose>
    </xsl:template>

    <xsl:template match=" hf:bool " mode="type">
        <xsl:text>bool</xsl:text>
    </xsl:template>

    <xsl:template match=" hf:binary " mode="type">
        <xsl:text>byte[]</xsl:text>
    </xsl:template>

    <xsl:template match=" hf:char " mode="type">
        <xsl:text>char</xsl:text>
    </xsl:template>

    <xsl:template match=" hf:string " mode="type">
        <xsl:text>string</xsl:text>

        <xsl:if test=" @array = 'true' ">
            <xsl:text>[]</xsl:text>
        </xsl:if>
    </xsl:template>

    <xsl:template match=" hf:short " mode="type">
        <xsl:text>short</xsl:text>
        <xsl:call-template name="type-native" />
    </xsl:template>

    <xsl:template match=" hf:int | hf:integer " mode="type">
        <xsl:text>int</xsl:text>
        <xsl:call-template name="type-native" />
    </xsl:template>

    <xsl:template match=" hf:long " mode="type">
        <xsl:text>long</xsl:text>
        <xsl:call-template name="type-native" />
    </xsl:template>

    <xsl:template match=" hf:float " mode="type">
        <xsl:text>float</xsl:text>
        <xsl:call-template name="type-native" />
    </xsl:template>

    <xsl:template match=" hf:double " mode="type">
        <xsl:text>double</xsl:text>
        <xsl:call-template name="type-native" />
    </xsl:template>

    <xsl:template match=" hf:decimal " mode="type">
        <xsl:text>decimal</xsl:text>
        <xsl:call-template name="type-native" />
    </xsl:template>

    <xsl:template match=" hf:date | hf:dateTime " mode="type">
        <xsl:text>DateTime</xsl:text>
        <xsl:call-template name="type-native" />
    </xsl:template>

    <xsl:template match=" hf:guid " mode="type">
        <xsl:text>Guid</xsl:text>

        <xsl:if test=" @array = 'true' ">
            <xsl:text>[]</xsl:text>
        </xsl:if>
    </xsl:template>

    <xsl:template match=" hf:binary " mode="type">
        <xsl:text>byte[]</xsl:text>
    </xsl:template>

    <xsl:template match=" hf:enumeration | hf:enumeration-ref " mode="type">
        <xsl:value-of select=" @type " />
    </xsl:template>

    <xsl:template match=" hf:structure | hf:structure-ref " mode="type">
        <xsl:value-of select=" @type " />

        <xsl:if test=" @array = 'true' ">
            <xsl:text>[]</xsl:text>
        </xsl:if>
    </xsl:template>



    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ hf:* / mode=type-attr
    ~ Responsible for returning all .NET attributes which need to markup
    ~ a property of the given type.
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" * " mode="type-attr" />

    <xsl:template match=" hf:date " mode="type-attr">
        <xsl:text>        [JsonConverter( typeof( Platinum.Json.DateConverter ) )]</xsl:text>
        <xsl:value-of select=" $NewLine " />

        <xsl:text>        [XmlElement( "xsd:date" )]</xsl:text>
        <xsl:value-of select=" $NewLine " />
    </xsl:template>

    <xsl:template match=" hf:date[ @optional = 'true' ] " mode="type-attr">
        <xsl:text>        [JsonConverter( typeof( Platinum.Json.NullableDateConverter ) )]</xsl:text>
        <xsl:value-of select=" $NewLine " />

        <xsl:text>        [XmlElement( "xsd:date" )]</xsl:text>
        <xsl:value-of select=" $NewLine " />
    </xsl:template>


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ hf:* / mode=rmap-attr
    ~ Responsible for returning all .NET attributes which need to markup
    ~ a property of the given type.
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" *[ @xls:cell ] " mode="rmap-attr">
        <xsl:text>        [Map( Expression = "</xsl:text>
        <xsl:value-of select=" @xls:cell " />
        <xsl:text>" )]</xsl:text>
        <xsl:value-of select=" $NewLine " />
    </xsl:template>
    


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ Functions
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template name="conditional">
        <xsl:choose>
            <xsl:when test=" @conditional = 'true' ">
                <xsl:text>true</xsl:text>
            </xsl:when>
            <xsl:otherwise>
                <xsl:text>false</xsl:text>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>


    <xsl:template name="substring-after-last">
        <xsl:param name="value"  />
        <xsl:param name="separator" select=" '.' " />

        <xsl:choose>
            <xsl:when test=" contains( $value, $separator ) ">
                <xsl:call-template name="substring-after-last">
                    <xsl:with-param name="value" select=" substring-after( $value, $separator ) " />
                    <xsl:with-param name="separator" select=" $separator " />
                </xsl:call-template>
            </xsl:when>

            <xsl:otherwise>
                <xsl:value-of select=" $value " />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

    <xsl:template name="summary">
        <xsl:param name="indent" />

        <xsl:choose>
            <xsl:when test=" hf:summary ">
                <xsl:value-of select=" $indent " />
                <xsl:text>/// &lt;summary&gt;</xsl:text>
                <!-- This new line not needed! -->

                <xsl:value-of select=" eo:ToWrap( hf:summary/text(), concat( $indent, '/// ' ), 80 ) "/>
                <xsl:text>
</xsl:text>

                <xsl:value-of select=" $indent " />
                <xsl:text>/// &lt;/summary&gt;</xsl:text>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select=" $indent " />
                <xsl:text>/// &lt;summary /&gt;</xsl:text>
            </xsl:otherwise>
        </xsl:choose>

    </xsl:template>

</xsl:stylesheet>
