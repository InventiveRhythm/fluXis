---@meta

---@alias Easing string
---| "None"
---| "In"
---| "Out"

---@param input AnchorName
---@return number
function Anchor(input) end

---@alias AnchorName string
---| "TopLeft"
---| "TopCentre"
---| "TopRight"
---| "CentreLeft"
---| "Centre"
---| "CentreRight"
---| "BottomLeft"
---| "BottomCentre"
---| "BottomRight"

---@alias ParameterDefinitionType string
---| "string"
---| "int"
---| "float"