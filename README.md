<p align="center">
  
  ![Cover](https://raw.githubusercontent.com/wiki/Clean-CaDET/platform/images/overview/cover.jpg)
  
</p>

<h1 align="center">DataSet Explorer</h1>
<div align="center">

  [![CodeFactor](https://www.codefactor.io/repository/github/clean-cadet/dataset-explorer/badge)](https://www.codefactor.io/repository/github/clean-cadet/dataset-explorer)
  [![Gitter](https://badges.gitter.im/Clean-CaDET/community.svg)](https://gitter.im/Clean-CaDET/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

</div>

<p align="justify">
  The DataSet Explorer (DSE) tool supports annotators during the code smell annotation procedure.
  </p>
<p align="justify">
  DSE tool development started as a part of the Clean CaDET project which was funded by the <a href="http://fondzanauku.gov.rs/?lang=en">Science Fund of the Republic of Serbia</a>.
</p>

- [Introduction](#introduction)
  - [What is the problem?](#what-is-the-problem)
  - [Who is it for?](#who-is-it-for)
- [Get started](https://github.com/Clean-CaDET/dataset-explorer/blob/master/SETUP.md)
- [Team](#team)

# Introduction

We outline notable resources that can assist researchers in using our implementation:
<ul>
  <li>Back-end source code (A repository hosting the source code of the DSE server application) -	https://github.com/Clean-CaDET/dataset-explorer </li>
  <li>Front-end source code	(A repository hosting the source code of the web UI) -https://github.com/Clean-CaDET/platform-explorer-ui-web </li>
  <li>General documentation	(A collection of wiki pages explaining our DSE design and supported features) -	https://github.com/Clean-CaDET/dataset-explorer/wiki </li>
</ul>



## What is the problem?
<p align="justify">
  DSE tool domain model corresponds to the conceptual model, ensuring that the tool’s implementation reflects the underlying conceptual model. The tool provides the analysis of code properties relevant to the several code smells, such as text(source code), structural metrics and relationships between system components.
</p>
<p align="justify">
  
</p>

## Who is it for?

### Researchers
<p align="justify">
To automate the process of dataset construction and analysis, we developed the <b>Dataset Explorer</b> tool.

The DSE tool supports the annotators during the training phase, enabling them to analyze the annotation schema within the tool, access the guidelines, perform POC annotation, and discuss disagreements on conflicting code snippets.

The DSE tool supports the annotators during the annotation phase, allowing simultaneous annotation of code snippets, checking if each code snippet is annotated by at least two annotators, and aggregating single labels assigned by multiple annotators to form the final label.

The DSE tool supported the resolution of the disagreements in the annotation phase by assigning conflicting code snippets to annotators that did not previously label them. In the discussion phase, the DSE tool identified conflicting code snippets that must be discussed and resolved.

The DSE tool enabled the removal of trivial code snippets based on the values of structural metrics. Removing trivial code snippets ensured that annotators were focused on code snippets that required deeper analysis. Next, the tool enabled the annotators to analyze various code properties: text (source code), structural metrics, and relationships between system components. Visualization of relationships between system components was realized as graphs: nodes representing components and edges representing their relationships (method invocation, inheritance, etc.). Presenting various code properties and heuristics helped annotators streamline their work. It eliminated the need for annotators to manually create their checklists or constantly refer to external resources, saving time and effort. 

DSE tool supports random sampling, allowing annotators to choose the number of code snippets that should be randomly extracted from the software project.

DSE tool allowed annotators to define the annotation schema. DSE tool supported annotators in analyzing relevant code properties.

For more details regarding the <b>Dataset Explorer</b>, check out the <a href="https://github.com/Clean-CaDET/platform/wiki/Module-Dataset-Explorer" target="_blank">module's page</a>
</p>

# Team
<p align="justify">
  Our project team consists of professors and teaching assistants from the Faculty of Technical Sciences, Novi Sad, Serbia. We are part of the Chair of Informatics, an organizational unit that has traditionally been the local center of excellence for both artificial intelligence and software engineering research.
</p>

- The people that make up the Clean CaDET Core are listed [here](https://clean-cadet.github.io/about/).
