# Not So Lucky Ninja

### Developers:

Don't commit to the **origin/Prod** branch.

The **origin/master** branch should be the main branch where the source code of HEAD always reflects a *production-ready* state. Only project leads should commit to this branch.

The **origin/Dev** branch should be the main branch where the source code of HEAD always reflects a state with the latest delivered development changes for the next release. This is where any automated/nightly builds could be built from.

Feature branches should be branched off **origin/Dev** and should be committed to the repository. Then a **pull request** should be submitted (against the **Dev** branch, not against the **master** branch). A typical feature branch naming convention would be the feature number that matches the issue number, followed by a description (such as *17_organize*).

Code review can be based off of the pull requests for each feature, to determine whether the issue has been completed in a satisfactory manner.

### Artists:

The **origin/Dev** branch should be where new and modified artwork and resources are committed.
