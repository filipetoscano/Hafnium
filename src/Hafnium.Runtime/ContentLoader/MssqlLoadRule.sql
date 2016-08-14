
select Content as Bytes, Version, LastModified
from HF_RULE
where Name = @RuleName
  and Variant is null
